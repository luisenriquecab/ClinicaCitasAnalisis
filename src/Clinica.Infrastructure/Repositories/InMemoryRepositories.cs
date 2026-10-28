using Clinica.Application.Interfaces.Repositories;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using System.Collections.Concurrent;

namespace Clinica.Infrastructure.Repositories;

public class InMemoryCitaRepository : ICitaRepository
{
    private readonly ConcurrentDictionary<Guid, Cita> _citas = new();

    public Task<Cita?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        _citas.TryGetValue(id, out var cita);
        return Task.FromResult(cita);
    }

    public Task<IEnumerable<Cita>> ObtenerPorPacienteIdAsync(Guid pacienteId, CancellationToken ct = default)
    {
        var list = _citas.Values.Where(c => c.PacienteId == pacienteId).OrderByDescending(c => c.FechaHoraInicio);
        return Task.FromResult<IEnumerable<Cita>>(list);
    }

    public Task<IEnumerable<Cita>> ObtenerPorMedicoYFechaAsync(Guid medicoId, DateTime fecha, CancellationToken ct = default)
    {
        var list = _citas.Values
            .Where(c => c.MedicoId == medicoId && c.FechaHoraInicio.Date == fecha.Date)
            .OrderBy(c => c.FechaHoraInicio);
        return Task.FromResult<IEnumerable<Cita>>(list);
    }

    public Task<IEnumerable<Cita>> ObtenerCitasEnRangoAsync(DateTime inicio, DateTime fin, CancellationToken ct = default)
    {
        var list = _citas.Values
            .Where(c => c.FechaHoraInicio >= inicio && c.FechaHoraInicio <= fin)
            .OrderBy(c => c.FechaHoraInicio);
        return Task.FromResult<IEnumerable<Cita>>(list);
    }

    public Task<bool> ExisteSolapamientoAsync(Guid medicoId, DateTime inicio, DateTime fin, Guid? excluirCitaId = null, CancellationToken ct = default)
    {
        var existe = _citas.Values.Any(c => 
            c.MedicoId == medicoId &&
            c.Estado != EstadoCita.Cancelada &&
            c.Estado != EstadoCita.CanceladaTardia &&
            (!excluirCitaId.HasValue || c.Id != excluirCitaId.Value) &&
            c.FechaHoraInicio < fin && 
            c.FechaHoraFin > inicio);

        return Task.FromResult(existe);
    }

    public Task<IEnumerable<Cita>> ObtenerPendientesRecordatorio24hAsync(DateTime inicioVentana, DateTime finVentana, CancellationToken ct = default)
    {
        var list = _citas.Values.Where(c => 
            c.Estado == EstadoCita.Programada || c.Estado == EstadoCita.Confirmada)
            .Where(c => !c.Recordatorio24hEnviado && c.FechaHoraInicio >= inicioVentana && c.FechaHoraInicio <= finVentana);

        return Task.FromResult<IEnumerable<Cita>>(list);
    }

    public Task<IEnumerable<Cita>> ObtenerPendientesRecordatorio2hAsync(DateTime inicioVentana, DateTime finVentana, CancellationToken ct = default)
    {
        var list = _citas.Values.Where(c => 
            c.Estado == EstadoCita.Programada || c.Estado == EstadoCita.Confirmada)
            .Where(c => !c.Recordatorio2hEnviado && c.FechaHoraInicio >= inicioVentana && c.FechaHoraInicio <= finVentana);

        return Task.FromResult<IEnumerable<Cita>>(list);
    }

    public Task AgregarAsync(Cita cita, CancellationToken ct = default)
    {
        _citas[cita.Id] = cita;
        return Task.CompletedTask;
    }

    public Task ActualizarAsync(Cita cita, CancellationToken ct = default)
    {
        _citas[cita.Id] = cita;
        return Task.CompletedTask;
    }
}

public class InMemoryMedicoRepository : IMedicoRepository
{
    private readonly List<Medico> _medicos;

    public InMemoryMedicoRepository()
    {
        // Médicos iniciales de prueba para la clínica
        _medicos = new List<Medico>
        {
            new Medico(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "COL-4521",
                "Dr. Roberto Mendizábal",
                "Cardiología",
                "dr.mendizabal@clinicaucentral.com",
                new TimeSpan(8, 0, 0),
                new TimeSpan(16, 0, 0),
                duracionCitaMinutos: 30
            ),
            new Medico(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "COL-7890",
                "Dra. Claudia Morales",
                "Pediatría",
                "dra.morales@clinicaucentral.com",
                new TimeSpan(9, 0, 0),
                new TimeSpan(17, 0, 0),
                duracionCitaMinutos: 30
            ),
            new Medico(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "COL-3141",
                "Dr. Juan Fernando Estrada",
                "Medicina General",
                "dr.estrada@clinicaucentral.com",
                new TimeSpan(8, 0, 0),
                new TimeSpan(18, 0, 0),
                duracionCitaMinutos: 20
            )
        };
    }

    public Task<Medico?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(_medicos.FirstOrDefault(m => m.Id == id));
    }

    public Task<IEnumerable<Medico>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<Medico>>(_medicos);
    }

    public Task<IEnumerable<Medico>> ObtenerPorEspecialidadAsync(string especialidad, CancellationToken ct = default)
    {
        var list = _medicos.Where(m => m.Especialidad.Equals(especialidad, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(list);
    }
}

public class InMemoryPacienteRepository : IPacienteRepository
{
    private readonly ConcurrentDictionary<Guid, Paciente> _pacientes = new();

    public InMemoryPacienteRepository()
    {
        var paciente1 = new Paciente(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "1234567890101",
            "Carmen Sandoval de León",
            "+502 5555-1234",
            "carmen.sandoval@correo.com",
            new DateTime(1963, 5, 14),
            PreferenciaNotificacion.WhatsApp
        );

        var paciente2 = new Paciente(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            "9876543210101",
            "Luis Enrique Cab",
            "+502 4444-9876",
            "luis.cab@correo.com",
            new DateTime(2003, 8, 20),
            PreferenciaNotificacion.PushNotification
        );

        _pacientes[paciente1.Id] = paciente1;
        _pacientes[paciente2.Id] = paciente2;
    }

    public Task<Paciente?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        _pacientes.TryGetValue(id, out var paciente);
        return Task.FromResult(paciente);
    }

    public Task<Paciente?> ObtenerPorDPIAsync(string dpi, CancellationToken ct = default)
    {
        var p = _pacientes.Values.FirstOrDefault(x => x.DPI == dpi.Trim());
        return Task.FromResult(p);
    }

    public Task AgregarAsync(Paciente paciente, CancellationToken ct = default)
    {
        _pacientes[paciente.Id] = paciente;
        return Task.CompletedTask;
    }
}
