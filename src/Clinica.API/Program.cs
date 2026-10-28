using Clinica.Application.DTOs;
using Clinica.Application.Interfaces.Repositories;
using Clinica.Application.Interfaces.Services;
using Clinica.Application.Services;
using Clinica.Infrastructure.Repositories;
using Clinica.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS para clientes Web, Móvil y Recepción
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registro de dependencias de Infraestructura (Persistencia y Repositorios)
builder.Services.AddSingleton<ICitaRepository, InMemoryCitaRepository>();
builder.Services.AddSingleton<IMedicoRepository, InMemoryMedicoRepository>();
builder.Services.AddSingleton<IPacienteRepository, InMemoryPacienteRepository>();

// Registro de dependencias de Dominio y Aplicación
builder.Services.AddScoped<IAgendaMedicaService, AgendaMedicaService>();
builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
builder.Services.AddScoped<ICitaManager, CitaManager>();
builder.Services.AddScoped<IInasistenciaTracker, InasistenciaTracker>();

// Registro del servicio de segundo plano (Recordatorios automáticos en segundo plano)
builder.Services.AddHostedService<RecordatoriosBackgroundWorker>();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Healthcheck & Banner informativo del sistema
app.MapGet("/", () => Results.Ok(new
{
    Proyecto = "Sistema de Control de Citas Clínicas - UMG",
    Curso = "Análisis de Sistemas II (037)",
    Catedratico = "Ing. José Eduardo Echeverria Ruíz",
    Arquitectura = "Clean Architecture + REST API",
    Estado = "Servicio Activo y Saludable",
    HoraServidor = DateTime.Now
}));

// ==========================================
// GRUPO DE RUTAS: CITAS MÉDICAS (/api/v1/citas)
// ==========================================
var citasApi = app.MapGroup("/api/v1/citas");

// 1. Agendar cita (Web, Móvil, Recepción)
citasApi.MapPost("/", async ([FromBody] SolicitudAgendarCitaDTO solicitud, ICitaManager manager) =>
{
    var resultado = await manager.AgendarCitaAsync(solicitud);
    if (resultado.IsFailure)
    {
        return resultado.ErrorCode == "CONFLICTO_HORARIO_OCUPADO"
            ? Results.Conflict(new { error = resultado.Error, codigo = resultado.ErrorCode })
            : Results.BadRequest(new { error = resultado.Error, codigo = resultado.ErrorCode });
    }
    return Results.Created($"/api/v1/citas/{resultado.Value!.Id}", resultado.Value);
});

// 2. Reprogramar cita
citasApi.MapPost("/reprogramar", async ([FromBody] SolicitudReprogramarCitaDTO solicitud, ICitaManager manager) =>
{
    var resultado = await manager.ReprogramarCitaAsync(solicitud);
    if (resultado.IsFailure)
    {
        return resultado.ErrorCode == "CONFLICTO_HORARIO_OCUPADO"
            ? Results.Conflict(new { error = resultado.Error, codigo = resultado.ErrorCode })
            : Results.BadRequest(new { error = resultado.Error, codigo = resultado.ErrorCode });
    }
    return Results.Ok(new { mensaje = "Cita reprogramada exitosamente." });
});

// 3. Cancelar cita
citasApi.MapPost("/cancelar", async ([FromBody] SolicitudCancelarCitaDTO solicitud, ICitaManager manager) =>
{
    var resultado = await manager.CancelarCitaAsync(solicitud);
    if (resultado.IsFailure)
    {
        return Results.BadRequest(new { error = resultado.Error, codigo = resultado.ErrorCode });
    }
    return Results.Ok(new { mensaje = "Cita cancelada correctamente." });
});

// 4. Confirmar cita
citasApi.MapPost("/{id:guid}/confirmar", async (Guid id, ICitaManager manager) =>
{
    var res = await manager.ConfirmarCitaAsync(id);
    return res.IsSuccess ? Results.Ok(new { mensaje = "Cita confirmada." }) : Results.BadRequest(new { error = res.Error });
});

// 5. Iniciar atención (Médico en consultorio)
citasApi.MapPost("/{id:guid}/iniciar-atencion", async (Guid id, ICitaManager manager) =>
{
    var res = await manager.IniciarAtencionAsync(id);
    return res.IsSuccess ? Results.Ok(new { mensaje = "Atención médica iniciada." }) : Results.BadRequest(new { error = res.Error });
});

// 6. Finalizar atención
citasApi.MapPost("/{id:guid}/finalizar-atencion", async (Guid id, ICitaManager manager) =>
{
    var res = await manager.FinalizarAtencionAsync(id);
    return res.IsSuccess ? Results.Ok(new { mensaje = "Atención médica finalizada." }) : Results.BadRequest(new { error = res.Error });
});

// 7. Marcar inasistencia (No-Show)
citasApi.MapPost("/{id:guid}/inasistencia", async (Guid id, [FromQuery] string observaciones, IInasistenciaTracker tracker) =>
{
    var res = await tracker.RegistrarInasistenciaAsync(id, observaciones ?? "Paciente no se presentó");
    return res.IsSuccess ? Results.Ok(new { mensaje = "Inasistencia registrada." }) : Results.BadRequest(new { error = res.Error });
});

// 8. Consultar citas por paciente
citasApi.MapGet("/paciente/{pacienteId:guid}", async (Guid pacienteId, ICitaManager manager) =>
{
    var citas = await manager.ObtenerCitasPorPacienteAsync(pacienteId);
    return Results.Ok(citas);
});

// 9. Consultar citas del día para Recepción
citasApi.MapGet("/dia", async ([FromQuery] DateTime? fecha, ICitaManager manager) =>
{
    var fechaConsulta = fecha ?? DateTime.Today;
    var citas = await manager.ObtenerCitasDelDiaAsync(fechaConsulta);
    return Results.Ok(citas);
});


// ==========================================
// GRUPO DE RUTAS: AGENDA MÉDICA (/api/v1/agenda)
// ==========================================
var agendaApi = app.MapGroup("/api/v1/agenda");

// 1. Obtener listado de médicos disponibles
agendaApi.MapGet("/medicos", async (IMedicoRepository repo) =>
{
    var medicos = await repo.ObtenerTodosAsync();
    return Results.Ok(medicos);
});

// 2. Obtener franjas horarias y disponibilidad de un médico
agendaApi.MapGet("/medico/{medicoId:guid}/disponibilidad", async (Guid medicoId, [FromQuery] DateTime? fecha, IAgendaMedicaService agendaService) =>
{
    var fechaConsulta = fecha ?? DateTime.Today.AddDays(1);
    var franjas = await agendaService.ObtenerFranjasDisponiblesAsync(medicoId, fechaConsulta);
    return Results.Ok(franjas);
});

// 3. Obtener agenda del médico para un día específico
agendaApi.MapGet("/medico/{medicoId:guid}/citas", async (Guid medicoId, [FromQuery] DateTime? fecha, ICitaManager manager) =>
{
    var fechaConsulta = fecha ?? DateTime.Today;
    var citas = await manager.ObtenerAgendaMedicoAsync(medicoId, fechaConsulta);
    return Results.Ok(citas);
});


// ==========================================
// GRUPO DE RUTAS: REPORTES (/api/v1/reportes)
// ==========================================
var reportesApi = app.MapGroup("/api/v1/reportes");

// Reporte analítico de inasistencias y no-shows
reportesApi.MapGet("/inasistencias", async (
    [FromQuery] DateTime? fechaInicio, 
    [FromQuery] DateTime? fechaFin, 
    [FromQuery] Guid? medicoId, 
    IInasistenciaTracker tracker) =>
{
    var inicio = fechaInicio ?? DateTime.Today.AddDays(-30);
    var fin = fechaFin ?? DateTime.Today;
    var reporte = await tracker.GenerarReporteInasistenciasAsync(inicio, fin, medicoId);
    return Results.Ok(reporte);
});


// ==========================================
// GRUPO DE RUTAS: NOTIFICACIONES (/api/v1/notificaciones)
// ==========================================
var notifApi = app.MapGroup("/api/v1/notificaciones");

// Disparador manual para demostración y evaluación docente
notifApi.MapPost("/procesar-pendientes", async (INotificationDispatcher notificador) =>
{
    int total = await notificador.ProcesarRecordatoriosPendientesAsync();
    return Results.Ok(new { mensaje = "Proceso de recordatorios ejecutado.", notificacionesEnviadas = total });
});

app.Run();
