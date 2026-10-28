using Clinica.Application.DTOs;
using Clinica.Application.Interfaces.Repositories;
using Clinica.Application.Interfaces.Services;
using Clinica.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Clinica.Infrastructure.Services;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly ICitaRepository _citaRepo;
    private readonly IPacienteRepository _pacienteRepo;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        ICitaRepository citaRepo, 
        IPacienteRepository pacienteRepo,
        ILogger<NotificationDispatcher> logger)
    {
        _citaRepo = citaRepo;
        _pacienteRepo = pacienteRepo;
        _logger = logger;
    }

    public async Task EnviarConfirmacionInmediataAsync(CitaDetalleDTO cita, CancellationToken ct = default)
    {
        var paciente = await _pacienteRepo.ObtenerPorIdAsync(cita.PacienteId, ct);
        var canal = paciente?.PreferenciaCanal.ToString() ?? "SMS/Email";

        _logger.LogInformation(
            "🔔 [CONFIRMACIÓN INMEDIATA] Enviada a {Paciente} ({Telefono}) vía {Canal}: Cita con {Medico} ({Especialidad}) el {Fecha:dd/MM/yyyy HH:mm} hrs.",
            cita.NombrePaciente, cita.TelefonoPaciente, canal, cita.NombreMedico, cita.EspecialidadMedico, cita.FechaHoraInicio);

        await Task.CompletedTask;
    }

    public async Task EnviarRecordatorioAsync(CitaDetalleDTO cita, TipoNotificacion tipo, CancellationToken ct = default)
    {
        var titulo = tipo switch
        {
            TipoNotificacion.Recordatorio24Horas => "⏰ [RECORDATORIO 24 HORAS]",
            TipoNotificacion.Recordatorio2Horas => "🚨 [RECORDATORIO URGENTE 2 HORAS]",
            TipoNotificacion.Reprogramacion => "🔄 [REPROGRAMACIÓN DE CITA]",
            TipoNotificacion.Cancelacion => "❌ [CANCELACIÓN DE CITA]",
            _ => "ℹ️ [NOTIFICACIÓN CLÍNICA]"
        };

        _logger.LogInformation(
            "{Titulo} Paciente: {Paciente} | Cita con {Medico} a las {Fecha:dd/MM/yyyy HH:mm} hrs.",
            titulo, cita.NombrePaciente, cita.NombreMedico, cita.FechaHoraInicio);

        await Task.CompletedTask;
    }

    public async Task<int> ProcesarRecordatoriosPendientesAsync(CancellationToken ct = default)
    {
        var ahora = DateTime.Now;
        int notificacionesProcesadas = 0;

        // 1. Escanear citas dentro de la ventana de 24 horas (entre 23h y 25h en el futuro)
        var inicio24h = ahora.AddHours(23);
        var fin24h = ahora.AddHours(25);
        var pendientes24h = await _citaRepo.ObtenerPendientesRecordatorio24hAsync(inicio24h, fin24h, ct);

        foreach (var cita in pendientes24h)
        {
            cita.RegistrarEnvioRecordatorio(TipoNotificacion.Recordatorio24Horas);
            await _citaRepo.ActualizarAsync(cita, ct);

            var paciente = await _pacienteRepo.ObtenerPorIdAsync(cita.PacienteId, ct);
            _logger.LogInformation("⏰ Auto-Recordatorio 24h despachado para cita {CitaId} del paciente {Paciente}",
                cita.Id, paciente?.NombreCompleto ?? cita.PacienteId.ToString());

            notificacionesProcesadas++;
        }

        // 2. Escanear citas dentro de la ventana de 2 horas (entre 1h 45m y 2h 15m)
        var inicio2h = ahora.AddMinutes(105);
        var fin2h = ahora.AddMinutes(135);
        var pendientes2h = await _citaRepo.ObtenerPendientesRecordatorio2hAsync(inicio2h, fin2h, ct);

        foreach (var cita in pendientes2h)
        {
            cita.RegistrarEnvioRecordatorio(TipoNotificacion.Recordatorio2Horas);
            await _citaRepo.ActualizarAsync(cita, ct);

            var paciente = await _pacienteRepo.ObtenerPorIdAsync(cita.PacienteId, ct);
            _logger.LogInformation("🚨 Auto-Recordatorio 2h despachado para cita {CitaId} del paciente {Paciente}",
                cita.Id, paciente?.NombreCompleto ?? cita.PacienteId.ToString());

            notificacionesProcesadas++;
        }

        return notificacionesProcesadas;
    }
}
