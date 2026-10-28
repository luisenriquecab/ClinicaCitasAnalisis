using Clinica.Application.Common;
using Clinica.Application.DTOs;
using Clinica.Application.Interfaces.Repositories;
using Clinica.Application.Interfaces.Services;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Domain.Exceptions;

namespace Clinica.Application.Services;

public class CitaManager : ICitaManager
{
    private readonly ICitaRepository _citaRepo;
    private readonly IMedicoRepository _medicoRepo;
    private readonly IPacienteRepository _pacienteRepo;
    private readonly IAgendaMedicaService _agendaService;
    private readonly INotificationDispatcher _notificador;

    public CitaManager(
        ICitaRepository citaRepo,
        IMedicoRepository medicoRepo,
        IPacienteRepository pacienteRepo,
        IAgendaMedicaService agendaService,
        INotificationDispatcher notificador)
    {
        _citaRepo = citaRepo;
        _medicoRepo = medicoRepo;
        _pacienteRepo = pacienteRepo;
        _agendaService = agendaService;
        _notificador = notificador;
    }

    public async Task<Result<CitaDetalleDTO>> AgendarCitaAsync(SolicitudAgendarCitaDTO solicitud, CancellationToken ct = default)
    {
        try
        {
            // 1. Validar existencia del paciente
            var paciente = await _pacienteRepo.ObtenerPorIdAsync(solicitud.PacienteId, ct);
            if (paciente == null || !paciente.Activo)
            {
                return Result<CitaDetalleDTO>.Failure("El paciente no existe o se encuentra inactivo.", "PACIENTE_NO_VALIDO");
            }

            // 2. Validar existencia del médico
            var medico = await _medicoRepo.ObtenerPorIdAsync(solicitud.MedicoId, ct);
            if (medico == null || !medico.Activo)
            {
                return Result<CitaDetalleDTO>.Failure("El médico seleccionado no existe o no está activo.", "MEDICO_NO_VALIDO");
            }

            // 3. Regla de Concurrencia / Disponibilidad
            var duracion = medico.DuracionCitaMinutos;
            var disponible = await _agendaService.MedicoTieneDisponibilidadAsync(
                solicitud.MedicoId, solicitud.FechaHoraInicio, duracion, null, ct);

            if (!disponible)
            {
                return Result<CitaDetalleDTO>.Failure(
                    "El horario seleccionado ya no se encuentra disponible. Por favor elija otra franja horaria.", 
                    "CONFLICTO_HORARIO_OCUPADO");
            }

            // 4. Crear entidad Cita aplicando invariantes de dominio
            var cita = Cita.Crear(
                solicitud.PacienteId,
                solicitud.MedicoId,
                solicitud.FechaHoraInicio,
                duracion,
                solicitud.MotivoConsulta,
                solicitud.CanalOrigen
            );

            // 5. Persistir en base de datos
            await _citaRepo.AgregarAsync(cita, ct);

            var detalleDTO = MapearADetalleDTO(cita, paciente, medico);

            // 6. Despacho asíncrono de confirmación
            await _notificador.EnviarConfirmacionInmediataAsync(detalleDTO, ct);

            return Result<CitaDetalleDTO>.Success(detalleDTO);
        }
        catch (ReglaDeNegocioException ex)
        {
            return Result<CitaDetalleDTO>.Failure(ex.Message, ex.CodigoError);
        }
        catch (Exception ex)
        {
            return Result<CitaDetalleDTO>.Failure($"Error al agendar la cita: {ex.Message}", "ERROR_INTERNO");
        }
    }

    public async Task<Result<bool>> ReprogramarCitaAsync(SolicitudReprogramarCitaDTO solicitud, CancellationToken ct = default)
    {
        try
        {
            var cita = await _citaRepo.ObtenerPorIdAsync(solicitud.CitaId, ct);
            if (cita == null)
            {
                return Result<bool>.Failure("La cita no existe.", "CITA_NO_ENCONTRADA");
            }

            var medico = await _medicoRepo.ObtenerPorIdAsync(cita.MedicoId, ct);
            if (medico == null)
            {
                return Result<bool>.Failure("Médico asociado no encontrado.", "MEDICO_NO_VALIDO");
            }

            // Validar disponibilidad excluyendo la cita actual
            var disponible = await _agendaService.MedicoTieneDisponibilidadAsync(
                cita.MedicoId, solicitud.NuevaFechaHoraInicio, medico.DuracionCitaMinutos, cita.Id, ct);

            if (!disponible)
            {
                return Result<bool>.Failure("El nuevo horario solicitado no se encuentra libre.", "CONFLICTO_HORARIO_OCUPADO");
            }

            cita.Reprogramar(solicitud.NuevaFechaHoraInicio, medico.DuracionCitaMinutos);
            await _citaRepo.ActualizarAsync(cita, ct);

            var paciente = await _pacienteRepo.ObtenerPorIdAsync(cita.PacienteId, ct);
            if (paciente != null)
            {
                var detalle = MapearADetalleDTO(cita, paciente, medico);
                await _notificador.EnviarRecordatorioAsync(detalle, TipoNotificacion.Reprogramacion, ct);
            }

            return Result<bool>.Success(true);
        }
        catch (ReglaDeNegocioException ex)
        {
            return Result<bool>.Failure(ex.Message, ex.CodigoError);
        }
    }

    public async Task<Result<bool>> CancelarCitaAsync(SolicitudCancelarCitaDTO solicitud, CancellationToken ct = default)
    {
        try
        {
            var cita = await _citaRepo.ObtenerPorIdAsync(solicitud.CitaId, ct);
            if (cita == null)
            {
                return Result<bool>.Failure("Cita no encontrada.", "CITA_NO_ENCONTRADA");
            }

            cita.Cancelar(solicitud.MotivoCancelacion, solicitud.CanceladoPor);
            await _citaRepo.ActualizarAsync(cita, ct);

            var paciente = await _pacienteRepo.ObtenerPorIdAsync(cita.PacienteId, ct);
            var medico = await _medicoRepo.ObtenerPorIdAsync(cita.MedicoId, ct);
            if (paciente != null && medico != null)
            {
                var detalle = MapearADetalleDTO(cita, paciente, medico);
                await _notificador.EnviarRecordatorioAsync(detalle, TipoNotificacion.Cancelacion, ct);
            }

            return Result<bool>.Success(true);
        }
        catch (ReglaDeNegocioException ex)
        {
            return Result<bool>.Failure(ex.Message, ex.CodigoError);
        }
    }

    public async Task<Result<bool>> ConfirmarCitaAsync(Guid citaId, CancellationToken ct = default)
    {
        var cita = await _citaRepo.ObtenerPorIdAsync(citaId, ct);
        if (cita == null) return Result<bool>.Failure("Cita no encontrada.");

        cita.Confirmar();
        await _citaRepo.ActualizarAsync(cita, ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> IniciarAtencionAsync(Guid citaId, CancellationToken ct = default)
    {
        var cita = await _citaRepo.ObtenerPorIdAsync(citaId, ct);
        if (cita == null) return Result<bool>.Failure("Cita no encontrada.");

        cita.IniciarAtencion();
        await _citaRepo.ActualizarAsync(cita, ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> FinalizarAtencionAsync(Guid citaId, CancellationToken ct = default)
    {
        var cita = await _citaRepo.ObtenerPorIdAsync(citaId, ct);
        if (cita == null) return Result<bool>.Failure("Cita no encontrada.");

        cita.FinalizarAtencion();
        await _citaRepo.ActualizarAsync(cita, ct);
        return Result<bool>.Success(true);
    }

    public async Task<IEnumerable<CitaDetalleDTO>> ObtenerCitasPorPacienteAsync(Guid pacienteId, CancellationToken ct = default)
    {
        var citas = await _citaRepo.ObtenerPorPacienteIdAsync(pacienteId, ct);
        return await MapearListaDetalleAsync(citas, ct);
    }

    public async Task<IEnumerable<CitaDetalleDTO>> ObtenerAgendaMedicoAsync(Guid medicoId, DateTime fecha, CancellationToken ct = default)
    {
        var citas = await _citaRepo.ObtenerPorMedicoYFechaAsync(medicoId, fecha.Date, ct);
        return await MapearListaDetalleAsync(citas, ct);
    }

    public async Task<IEnumerable<CitaDetalleDTO>> ObtenerCitasDelDiaAsync(DateTime fecha, CancellationToken ct = default)
    {
        var inicio = fecha.Date;
        var fin = inicio.AddDays(1).AddTicks(-1);
        var citas = await _citaRepo.ObtenerCitasEnRangoAsync(inicio, fin, ct);
        return await MapearListaDetalleAsync(citas, ct);
    }

    private async Task<List<CitaDetalleDTO>> MapearListaDetalleAsync(IEnumerable<Cita> citas, CancellationToken ct)
    {
        var lista = new List<CitaDetalleDTO>();
        foreach (var c in citas)
        {
            var p = await _pacienteRepo.ObtenerPorIdAsync(c.PacienteId, ct);
            var m = await _medicoRepo.ObtenerPorIdAsync(c.MedicoId, ct);
            if (p != null && m != null)
            {
                lista.Add(MapearADetalleDTO(c, p, m));
            }
        }
        return lista;
    }

    private static CitaDetalleDTO MapearADetalleDTO(Cita cita, Paciente paciente, Medico medico)
    {
        return new CitaDetalleDTO(
            cita.Id,
            paciente.Id,
            paciente.NombreCompleto,
            paciente.Telefono,
            medico.Id,
            medico.NombreCompleto,
            medico.Especialidad,
            cita.FechaHoraInicio,
            cita.FechaHoraFin,
            cita.Estado,
            cita.Estado.ToString(),
            cita.CanalOrigen,
            cita.MotivoConsulta,
            cita.Recordatorio24hEnviado,
            cita.Recordatorio2hEnviado,
            cita.AplicaCargoPorCancelacionTardia
        );
    }
}
