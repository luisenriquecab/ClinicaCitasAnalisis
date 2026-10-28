using Clinica.Domain.Entities;

namespace Clinica.Application.Interfaces.Repositories;

public interface ICitaRepository
{
    Task<Cita?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Cita>> ObtenerPorPacienteIdAsync(Guid pacienteId, CancellationToken ct = default);
    Task<IEnumerable<Cita>> ObtenerPorMedicoYFechaAsync(Guid medicoId, DateTime fecha, CancellationToken ct = default);
    Task<IEnumerable<Cita>> ObtenerCitasEnRangoAsync(DateTime inicio, DateTime fin, CancellationToken ct = default);
    Task<bool> ExisteSolapamientoAsync(Guid medicoId, DateTime inicio, DateTime fin, Guid? excluirCitaId = null, CancellationToken ct = default);
    Task<IEnumerable<Cita>> ObtenerPendientesRecordatorio24hAsync(DateTime inicioVentana, DateTime finVentana, CancellationToken ct = default);
    Task<IEnumerable<Cita>> ObtenerPendientesRecordatorio2hAsync(DateTime inicioVentana, DateTime finVentana, CancellationToken ct = default);
    Task AgregarAsync(Cita cita, CancellationToken ct = default);
    Task ActualizarAsync(Cita cita, CancellationToken ct = default);
}

public interface IMedicoRepository
{
    Task<Medico?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Medico>> ObtenerTodosAsync(CancellationToken ct = default);
    Task<IEnumerable<Medico>> ObtenerPorEspecialidadAsync(string especialidad, CancellationToken ct = default);
}

public interface IPacienteRepository
{
    Task<Paciente?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Paciente?> ObtenerPorDPIAsync(string dpi, CancellationToken ct = default);
    Task AgregarAsync(Paciente paciente, CancellationToken ct = default);
}
