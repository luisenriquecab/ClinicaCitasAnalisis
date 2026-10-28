using Clinica.Application.DTOs;
using Clinica.Application.Services;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Infrastructure.Repositories;
using Clinica.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Clinica.UnitTests;

public class AgendaAndConcurrencyTests
{
    private readonly InMemoryCitaRepository _citaRepo = new();
    private readonly InMemoryMedicoRepository _medicoRepo = new();
    private readonly InMemoryPacienteRepository _pacienteRepo = new();
    private readonly AgendaMedicaService _agendaService;
    private readonly NotificationDispatcher _notificador;
    private readonly CitaManager _citaManager;
    private readonly InasistenciaTracker _tracker;

    public AgendaAndConcurrencyTests()
    {
        _agendaService = new AgendaMedicaService(_medicoRepo, _citaRepo);
        _notificador = new NotificationDispatcher(_citaRepo, _pacienteRepo, NullLogger<NotificationDispatcher>.Instance);
        _citaManager = new CitaManager(_citaRepo, _medicoRepo, _pacienteRepo, _agendaService, _notificador);
        _tracker = new InasistenciaTracker(_citaRepo, _medicoRepo);
    }

    [Fact]
    public async Task AgendarCita_HorarioDisponible_RetornaExitoYNotifica()
    {
        // Arrange
        var medicoId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Dr. Mendizábal (08:00 - 16:00)
        var pacienteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // Doña Carmen
        
        // Próximo martes a las 10:00 AM
        var fechaMartes = DateTime.Today.AddDays(((int)DayOfWeek.Tuesday - (int)DateTime.Today.DayOfWeek + 7) % 7 + 7).AddHours(10);

        var solicitud = new SolicitudAgendarCitaDTO(
            pacienteId,
            medicoId,
            fechaMartes,
            "Consulta de cardiología",
            CanalOrigen.Web
        );

        // Act
        var res = await _citaManager.AgendarCitaAsync(solicitud);

        // Assert
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Value);
        Assert.Equal("Dr. Roberto Mendizábal", res.Value.NombreMedico);
        Assert.Equal("Carmen Sandoval de León", res.Value.NombrePaciente);
    }

    [Fact]
    public async Task AgendarCita_MismoMedicoMismoHorarioConcurrente_SegundoIntentoDebeSerRechazado()
    {
        // Arrange
        var medicoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var paciente1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var paciente2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var fechaMiercoles = DateTime.Today.AddDays(((int)DayOfWeek.Wednesday - (int)DateTime.Today.DayOfWeek + 7) % 7 + 7).AddHours(11);

        var solicitud1 = new SolicitudAgendarCitaDTO(paciente1, medicoId, fechaMiercoles, "Primera cita", CanalOrigen.Movil);
        var solicitud2 = new SolicitudAgendarCitaDTO(paciente2, medicoId, fechaMiercoles, "Segunda cita simultánea", CanalOrigen.Recepcion);

        // Act
        var res1 = await _citaManager.AgendarCitaAsync(solicitud1);
        var res2 = await _citaManager.AgendarCitaAsync(solicitud2);

        // Assert
        Assert.True(res1.IsSuccess, "La primera reserva debió aceptarse");
        Assert.True(res2.IsFailure, "La segunda reserva debió rechazarse por colisión");
        Assert.Equal("CONFLICTO_HORARIO_OCUPADO", res2.ErrorCode);
    }

    [Fact]
    public async Task ReporteInasistencias_CalculaPorcentajesYMetricasConPrecision()
    {
        // Arrange
        var medicoId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Dra. Morales
        var pacienteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var baseFecha = DateTime.Today.AddDays(-5);

        // Crear 4 citas históricas en el repositorio
        var c1 = Cita.CrearHistorica(Guid.NewGuid(), pacienteId, medicoId, baseFecha.AddHours(9), 30, EstadoCita.Inasistencia, "Cita 1");
        var c2 = Cita.CrearHistorica(Guid.NewGuid(), pacienteId, medicoId, baseFecha.AddHours(10), 30, EstadoCita.Atendida, "Cita 2");
        var c3 = Cita.CrearHistorica(Guid.NewGuid(), pacienteId, medicoId, baseFecha.AddHours(11), 30, EstadoCita.Atendida, "Cita 3");
        var c4 = Cita.CrearHistorica(Guid.NewGuid(), pacienteId, medicoId, baseFecha.AddHours(12), 30, EstadoCita.Atendida, "Cita 4");

        await _citaRepo.AgregarAsync(c1);
        await _citaRepo.AgregarAsync(c2);
        await _citaRepo.AgregarAsync(c3);
        await _citaRepo.AgregarAsync(c4);

        // Act
        var reporte = await _tracker.GenerarReporteInasistenciasAsync(baseFecha.AddDays(-1), DateTime.Today);

        // Assert: 1 inasistencia de 4 citas = 25.0%
        Assert.Equal(4, reporte.TotalCitasProgramadas);
        Assert.Equal(1, reporte.TotalInasistencias);
        Assert.Equal(25.0, reporte.TasaGlobalInasistenciaPorcentaje);
    }
}
