using Clinica.Application.DTOs;
using Clinica.Application.Services;
using Clinica.Domain.Enums;
using Clinica.Infrastructure.Repositories;
using Clinica.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Clinica.UnitTests;

public class SeguridadTests
{
    private readonly InMemoryCitaRepository _citaRepo = new();
    private readonly InMemoryMedicoRepository _medicoRepo = new();
    private readonly InMemoryPacienteRepository _pacienteRepo = new();
    private readonly AgendaMedicaService _agendaService;
    private readonly NotificationDispatcher _notificador;
    private readonly CitaManager _citaManager;

    public SeguridadTests()
    {
        _agendaService = new AgendaMedicaService(_medicoRepo, _citaRepo);
        _notificador = new NotificationDispatcher(_citaRepo, _pacienteRepo, NullLogger<NotificationDispatcher>.Instance);
        _citaManager = new CitaManager(_citaRepo, _medicoRepo, _pacienteRepo, _agendaService, _notificador);
    }

    [Fact]
    public async Task Seguridad_InyeccionSQLEnMotivoConsulta_EsTratadoComoTextoPlanoYNoCorrompeElSistema()
    {
        // Arrange: Carga maliciosa típica de SQL Injection
        var sqlInjectionPayload = "Consulta de control'; DROP TABLE Citas; --";
        var pacienteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var medicoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var fechaFutura = DateTime.Today.AddDays(7).AddHours(9);

        var solicitud = new SolicitudAgendarCitaDTO(
            pacienteId,
            medicoId,
            fechaFutura,
            sqlInjectionPayload,
            CanalOrigen.Web
        );

        // Act
        var res = await _citaManager.AgendarCitaAsync(solicitud);

        // Assert: El sistema debe procesar el texto sin interpretarlo como comando SQL
        Assert.True(res.IsSuccess);
        Assert.Equal(sqlInjectionPayload, res.Value!.MotivoConsulta);

        // Comprobar que el repositorio sigue vivo y con los datos íntegros
        var citasPaciente = await _citaManager.ObtenerCitasPorPacienteAsync(pacienteId);
        Assert.Contains(citasPaciente, c => c.Id == res.Value.Id);
    }

    [Fact]
    public async Task Seguridad_PrivacidadEntrePacientes_PacienteSoloPuedeVerSusPropiasCitas()
    {
        // Arrange
        var paciente1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var paciente2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var medicoId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var fecha1 = DateTime.Today.AddDays(8).AddHours(9);
        var fecha2 = DateTime.Today.AddDays(8).AddHours(10);

        await _citaManager.AgendarCitaAsync(new SolicitudAgendarCitaDTO(paciente1, medicoId, fecha1, "Cita Privada 1"));
        await _citaManager.AgendarCitaAsync(new SolicitudAgendarCitaDTO(paciente2, medicoId, fecha2, "Cita Privada 2"));

        // Act
        var citasPaciente1 = (await _citaManager.ObtenerCitasPorPacienteAsync(paciente1)).ToList();
        var citasPaciente2 = (await _citaManager.ObtenerCitasPorPacienteAsync(paciente2)).ToList();

        // Assert: Aislamiento total de información médica confidencial
        Assert.All(citasPaciente1, c => Assert.Equal(paciente1, c.PacienteId));
        Assert.DoesNotContain(citasPaciente1, c => c.PacienteId == paciente2);

        Assert.All(citasPaciente2, c => Assert.Equal(paciente2, c.PacienteId));
        Assert.DoesNotContain(citasPaciente2, c => c.PacienteId == paciente1);
    }
}
