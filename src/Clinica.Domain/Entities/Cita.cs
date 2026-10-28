using Clinica.Domain.Enums;
using Clinica.Domain.Exceptions;

namespace Clinica.Domain.Entities;

public class Cita
{
    public Guid Id { get; private set; }
    public Guid PacienteId { get; private set; }
    public Guid MedicoId { get; private set; }
    public DateTime FechaHoraInicio { get; private set; }
    public DateTime FechaHoraFin { get; private set; }
    public EstadoCita Estado { get; private set; }
    public CanalOrigen CanalOrigen { get; private set; }
    public string MotivoConsulta { get; private set; } = string.Empty;
    public bool Recordatorio24hEnviado { get; private set; }
    public bool Recordatorio2hEnviado { get; private set; }
    public string MotivoCancelacion { get; private set; } = string.Empty;
    public string CanceladoPor { get; private set; } = string.Empty;
    public bool AplicaCargoPorCancelacionTardia { get; private set; }
    public string ObservacionesInasistencia { get; private set; } = string.Empty;
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaModificacion { get; private set; }

    private Cita() { }

    public static Cita Crear(
        Guid pacienteId, 
        Guid medicoId, 
        DateTime fechaHoraInicio, 
        int duracionMinutos, 
        string motivoConsulta, 
        CanalOrigen canalOrigen)
    {
        if (pacienteId == Guid.Empty) throw new ReglaDeNegocioException("Debe especificarse un paciente válido.");
        if (medicoId == Guid.Empty) throw new ReglaDeNegocioException("Debe especificarse un médico válido.");
        if (fechaHoraInicio <= DateTime.Now) throw new ReglaDeNegocioException("No es posible programar citas en una fecha y hora pasada.");
        if (duracionMinutos <= 0) throw new ReglaDeNegocioException("La duración debe ser mayor a 0.");
        if (string.IsNullOrWhiteSpace(motivoConsulta)) throw new ReglaDeNegocioException("El motivo de consulta es requerido.");

        return new Cita
        {
            Id = Guid.NewGuid(),
            PacienteId = pacienteId,
            MedicoId = medicoId,
            FechaHoraInicio = fechaHoraInicio,
            FechaHoraFin = fechaHoraInicio.AddMinutes(duracionMinutos),
            Estado = EstadoCita.Programada,
            CanalOrigen = canalOrigen,
            MotivoConsulta = motivoConsulta.Trim(),
            Recordatorio24hEnviado = false,
            Recordatorio2hEnviado = false,
            MotivoCancelacion = string.Empty,
            CanceladoPor = string.Empty,
            AplicaCargoPorCancelacionTardia = false,
            ObservacionesInasistencia = string.Empty,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static Cita CrearHistorica(
        Guid id,
        Guid pacienteId,
        Guid medicoId,
        DateTime fechaHoraInicio,
        int duracionMinutos,
        EstadoCita estado,
        string motivoConsulta = "Consulta migrada")
    {
        return new Cita
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            PacienteId = pacienteId,
            MedicoId = medicoId,
            FechaHoraInicio = fechaHoraInicio,
            FechaHoraFin = fechaHoraInicio.AddMinutes(duracionMinutos),
            Estado = estado,
            CanalOrigen = CanalOrigen.Recepcion,
            MotivoConsulta = motivoConsulta,
            FechaCreacion = fechaHoraInicio.AddDays(-2)
        };
    }

    public void Confirmar()
    {
        if (Estado == EstadoCita.Cancelada || Estado == EstadoCita.CanceladaTardia || Estado == EstadoCita.Inasistencia)
        {
            throw new ReglaDeNegocioException($"No se puede confirmar una cita en estado {Estado}.");
        }
        Estado = EstadoCita.Confirmada;
        FechaModificacion = DateTime.UtcNow;
    }

    public void Reprogramar(DateTime nuevaFechaHoraInicio, int duracionMinutos)
    {
        if (nuevaFechaHoraInicio <= DateTime.Now)
        {
            throw new ReglaDeNegocioException("La nueva fecha debe ser futura.");
        }
        if (Estado == EstadoCita.Atendida || Estado == EstadoCita.Inasistencia)
        {
            throw new ReglaDeNegocioException($"No es posible reprogramar una cita {Estado}.");
        }

        FechaHoraInicio = nuevaFechaHoraInicio;
        FechaHoraFin = nuevaFechaHoraInicio.AddMinutes(duracionMinutos);
        Estado = EstadoCita.Programada;
        Recordatorio24hEnviado = false;
        Recordatorio2hEnviado = false;
        FechaModificacion = DateTime.UtcNow;
    }

    public void Cancelar(string motivo, string canceladoPor)
    {
        if (Estado == EstadoCita.Atendida || Estado == EstadoCita.Cancelada || Estado == EstadoCita.CanceladaTardia)
        {
            throw new ReglaDeNegocioException($"La cita ya se encuentra en estado {Estado}.");
        }

        var horasRestantes = (FechaHoraInicio - DateTime.Now).TotalHours;
        if (horasRestantes < 2)
        {
            Estado = EstadoCita.CanceladaTardia;
            AplicaCargoPorCancelacionTardia = true;
        }
        else
        {
            Estado = EstadoCita.Cancelada;
            AplicaCargoPorCancelacionTardia = false;
        }

        MotivoCancelacion = motivo;
        CanceladoPor = canceladoPor;
        FechaModificacion = DateTime.UtcNow;
    }

    public void IniciarAtencion()
    {
        if (Estado != EstadoCita.Confirmada && Estado != EstadoCita.Programada)
        {
            throw new ReglaDeNegocioException("Solo citas programadas o confirmadas pueden iniciar consulta.");
        }
        Estado = EstadoCita.EnAtencion;
        FechaModificacion = DateTime.UtcNow;
    }

    public void FinalizarAtencion()
    {
        if (Estado != EstadoCita.EnAtencion)
        {
            throw new ReglaDeNegocioException("La cita no se encuentra en curso de atención.");
        }
        Estado = EstadoCita.Atendida;
        FechaModificacion = DateTime.UtcNow;
    }

    public void MarcarInasistencia(string observaciones = "")
    {
        if (Estado == EstadoCita.Atendida || Estado == EstadoCita.Cancelada || Estado == EstadoCita.CanceladaTardia)
        {
            throw new ReglaDeNegocioException($"No se puede marcar inasistencia para una cita {Estado}.");
        }
        Estado = EstadoCita.Inasistencia;
        ObservacionesInasistencia = observaciones;
        FechaModificacion = DateTime.UtcNow;
    }

    public void RegistrarEnvioRecordatorio(TipoNotificacion tipo)
    {
        if (tipo == TipoNotificacion.Recordatorio24Horas)
        {
            Recordatorio24hEnviado = true;
        }
        else if (tipo == TipoNotificacion.Recordatorio2Horas)
        {
            Recordatorio2hEnviado = true;
        }
        FechaModificacion = DateTime.UtcNow;
    }
}
