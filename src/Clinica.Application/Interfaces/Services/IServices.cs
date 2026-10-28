using Clinica.Application.Common;
using Clinica.Application.DTOs;
using Clinica.Domain.Enums;

namespace Clinica.Application.Interfaces.Services;

public interface ICitaManager
{
    Task<Result<CitaDetalleDTO>> AgendarCitaAsync(SolicitudAgendarCitaDTO solicitud, CancellationToken ct = default);
    Task<Result<bool>> ReprogramarCitaAsync(SolicitudReprogramarCitaDTO solicitud, CancellationToken ct = default);
    Task<Result<bool>> CancelarCitaAsync(SolicitudCancelarCitaDTO solicitud, CancellationToken ct = default);
    Task<Result<bool>> ConfirmarCitaAsync(Guid citaId, CancellationToken ct = default);
    Task<Result<bool>> IniciarAtencionAsync(Guid citaId, CancellationToken ct = default);
    Task<Result<bool>> FinalizarAtencionAsync(Guid citaId, CancellationToken ct = default);
    Task<IEnumerable<CitaDetalleDTO>> ObtenerCitasPorPacienteAsync(Guid pacienteId, CancellationToken ct = default);
    Task<IEnumerable<CitaDetalleDTO>> ObtenerAgendaMedicoAsync(Guid medicoId, DateTime fecha, CancellationToken ct = default);
    Task<IEnumerable<CitaDetalleDTO>> ObtenerCitasDelDiaAsync(DateTime fecha, CancellationToken ct = default);
}

public interface IAgendaMedicaService
{
    Task<bool> MedicoTieneDisponibilidadAsync(Guid medicoId, DateTime fechaHora, int duracionMinutos, Guid? excluirCitaId = null, CancellationToken ct = default);
    Task<IEnumerable<FranjaHorariaDTO>> ObtenerFranjasDisponiblesAsync(Guid medicoId, DateTime fecha, CancellationToken ct = default);
}

public interface INotificationDispatcher
{
    Task EnviarConfirmacionInmediataAsync(CitaDetalleDTO cita, CancellationToken ct = default);
    Task EnviarRecordatorioAsync(CitaDetalleDTO cita, TipoNotificacion tipo, CancellationToken ct = default);
    Task<int> ProcesarRecordatoriosPendientesAsync(CancellationToken ct = default);
}

public interface IInasistenciaTracker
{
    Task<Result<bool>> RegistrarInasistenciaAsync(Guid citaId, string observaciones, CancellationToken ct = default);
    Task<ReporteInasistenciasDTO> GenerarReporteInasistenciasAsync(DateTime fechaInicio, DateTime fechaFin, Guid? medicoId = null, CancellationToken ct = default);
}
