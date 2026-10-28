using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Domain.Exceptions;
using Xunit;

namespace Clinica.UnitTests;

public class CitaDomainTests
{
    private readonly Guid _pacienteId = Guid.NewGuid();
    private readonly Guid _medicoId = Guid.NewGuid();

    [Fact]
    public void CrearCita_ConFechaPasada_DebeLanzarExcepcionDeDominio()
    {
        // Arrange
        var fechaPasada = DateTime.Now.AddHours(-1);

        // Act & Assert
        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Cita.Crear(_pacienteId, _medicoId, fechaPasada, 30, "Control mensual", CanalOrigen.Web));

        Assert.Contains("No es posible programar citas en una fecha y hora pasada", ex.Message);
    }

    [Fact]
    public void CrearCita_ConDatosValidos_EstadoInicialDebeSerProgramada()
    {
        // Arrange
        var fechaFutura = DateTime.Now.AddDays(2).Date.AddHours(10);

        // Act
        var cita = Cita.Crear(_pacienteId, _medicoId, fechaFutura, 30, "Chequeo de rutina", CanalOrigen.Movil);

        // Assert
        Assert.NotNull(cita);
        Assert.Equal(EstadoCita.Programada, cita.Estado);
        Assert.Equal(fechaFutura.AddMinutes(30), cita.FechaHoraFin);
        Assert.False(cita.Recordatorio24hEnviado);
        Assert.False(cita.Recordatorio2hEnviado);
        Assert.False(cita.AplicaCargoPorCancelacionTardia);
    }

    [Fact]
    public void CancelarCita_ConMenosDeDosHorasDeAntelacion_DebeMarcarCancelacionTardiaYPenalizacion()
    {
        // Arrange: Cita en 1 hora
        var fechaInminente = DateTime.Now.AddHours(1);
        var cita = Cita.Crear(_pacienteId, _medicoId, fechaInminente, 30, "Urgencia dental", CanalOrigen.Recepcion);

        // Act
        cita.Cancelar("No podré llegar por tráfico", "Paciente");

        // Assert
        Assert.Equal(EstadoCita.CanceladaTardia, cita.Estado);
        Assert.True(cita.AplicaCargoPorCancelacionTardia);
        Assert.Equal("No podré llegar por tráfico", cita.MotivoCancelacion);
    }

    [Fact]
    public void CancelarCita_ConMasDeDosHorasDeAntelacion_DebeCancelarSinPenalizacion()
    {
        // Arrange: Cita en 48 horas
        var fechaFutura = DateTime.Now.AddDays(2);
        var cita = Cita.Crear(_pacienteId, _medicoId, fechaFutura, 30, "Consulta dermatológica", CanalOrigen.Web);

        // Act
        cita.Cancelar("Viaje imprevisto", "Paciente");

        // Assert
        Assert.Equal(EstadoCita.Cancelada, cita.Estado);
        Assert.False(cita.AplicaCargoPorCancelacionTardia);
    }

    [Fact]
    public void ReprogramarCita_ConFechaFutura_DebeActualizarHorarioYReiniciarBanderasDeRecordatorio()
    {
        // Arrange
        var fechaOriginal = DateTime.Now.AddDays(1);
        var cita = Cita.Crear(_pacienteId, _medicoId, fechaOriginal, 30, "Pediatría", CanalOrigen.Movil);
        cita.RegistrarEnvioRecordatorio(TipoNotificacion.Recordatorio24Horas);

        var nuevoHorario = DateTime.Now.AddDays(4).Date.AddHours(14);

        // Act
        cita.Reprogramar(nuevoHorario, 30);

        // Assert
        Assert.Equal(nuevoHorario, cita.FechaHoraInicio);
        Assert.Equal(EstadoCita.Programada, cita.Estado);
        Assert.False(cita.Recordatorio24hEnviado);
    }

    [Fact]
    public void FlujoAtencionMedica_IniciarYFinalizar_DebeTransicionarEstadosCorrectamente()
    {
        // Arrange
        var cita = Cita.Crear(_pacienteId, _medicoId, DateTime.Now.AddDays(1), 30, "Control", CanalOrigen.Web);
        cita.Confirmar();

        // Act & Assert 1: Iniciar
        cita.IniciarAtencion();
        Assert.Equal(EstadoCita.EnAtencion, cita.Estado);

        // Act & Assert 2: Finalizar
        cita.FinalizarAtencion();
        Assert.Equal(EstadoCita.Atendida, cita.Estado);
    }

    [Fact]
    public void MarcarInasistencia_DebeCambiarEstadoAInasistencia()
    {
        // Arrange
        var cita = Cita.Crear(_pacienteId, _medicoId, DateTime.Now.AddDays(1), 30, "Control", CanalOrigen.Web);

        // Act
        cita.MarcarInasistencia("Paciente no contestó llamadas de recepción");

        // Assert
        Assert.Equal(EstadoCita.Inasistencia, cita.Estado);
        Assert.Equal("Paciente no contestó llamadas de recepción", cita.ObservacionesInasistencia);
    }
}
