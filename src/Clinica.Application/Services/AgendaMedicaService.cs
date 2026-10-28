using Clinica.Application.DTOs;
using Clinica.Application.Interfaces.Repositories;
using Clinica.Application.Interfaces.Services;
using Clinica.Domain.Enums;

namespace Clinica.Application.Services;

public class AgendaMedicaService : IAgendaMedicaService
{
    private readonly IMedicoRepository _medicoRepo;
    private readonly ICitaRepository _citaRepo;

    public AgendaMedicaService(IMedicoRepository medicoRepo, ICitaRepository citaRepo)
    {
        _medicoRepo = medicoRepo;
        _citaRepo = citaRepo;
    }

    public async Task<bool> MedicoTieneDisponibilidadAsync(
        Guid medicoId, 
        DateTime fechaHora, 
        int duracionMinutos, 
        Guid? excluirCitaId = null, 
        CancellationToken ct = default)
    {
        var medico = await _medicoRepo.ObtenerPorIdAsync(medicoId, ct);
        if (medico == null || !medico.Activo)
        {
            return false;
        }

        // 1. Validar que la cita esté dentro del horario laboral del médico
        if (!medico.EstaEnHorarioLaboral(fechaHora, duracionMinutos))
        {
            return false;
        }

        // 2. Validar que no caiga en fin de semana (política de la clínica: Lunes a Viernes)
        if (fechaHora.DayOfWeek == DayOfWeek.Saturday || fechaHora.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        // 3. Validar solapamiento con otras citas del médico en base de datos
        var fin = fechaHora.AddMinutes(duracionMinutos);
        var solapado = await _citaRepo.ExisteSolapamientoAsync(medicoId, fechaHora, fin, excluirCitaId, ct);

        return !solapado;
    }

    public async Task<IEnumerable<FranjaHorariaDTO>> ObtenerFranjasDisponiblesAsync(
        Guid medicoId, 
        DateTime fecha, 
        CancellationToken ct = default)
    {
        var medico = await _medicoRepo.ObtenerPorIdAsync(medicoId, ct);
        if (medico == null || !medico.Activo)
        {
            return Enumerable.Empty<FranjaHorariaDTO>();
        }

        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
        {
            return Enumerable.Empty<FranjaHorariaDTO>();
        }

        var citasDelDia = await _citaRepo.ObtenerPorMedicoYFechaAsync(medicoId, fecha.Date, ct);
        var citasActivas = citasDelDia
            .Where(c => c.Estado != EstadoCita.Cancelada && c.Estado != EstadoCita.CanceladaTardia)
            .ToList();

        var franjas = new List<FranjaHorariaDTO>();
        var horaActual = fecha.Date.Add(medico.HoraInicioJornada);
        var horaFinJornada = fecha.Date.Add(medico.HoraFinJornada);
        var duracion = medico.DuracionCitaMinutos;

        while (horaActual.AddMinutes(duracion) <= horaFinJornada)
        {
            var franjaFin = horaActual.AddMinutes(duracion);
            var ocupada = citasActivas.Any(c => c.FechaHoraInicio < franjaFin && c.FechaHoraFin > horaActual);

            var disponible = !ocupada && horaActual > DateTime.Now;
            var motivo = ocupada ? "Horario reservado por otro paciente" : (horaActual <= DateTime.Now ? "Horario ya transcurrido" : "");

            franjas.Add(new FranjaHorariaDTO(horaActual, franjaFin, disponible, motivo));
            horaActual = franjaFin;
        }

        return franjas;
    }
}
