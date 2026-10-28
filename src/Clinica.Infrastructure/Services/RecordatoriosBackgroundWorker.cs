using Clinica.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Clinica.Infrastructure.Services;

public class RecordatoriosBackgroundWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecordatoriosBackgroundWorker> _logger;
    private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(30);

    public RecordatoriosBackgroundWorker(
        IServiceScopeFactory scopeFactory, 
        ILogger<RecordatoriosBackgroundWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 [WORKER INICIADO] Demonio en segundo plano para escaneo y despacho de recordatorios clínicos activo.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var notificador = scope.ServiceProvider.GetRequiredService<INotificationDispatcher>();
                    int procesados = await notificador.ProcesarRecordatoriosPendientesAsync(stoppingToken);
                    if (procesados > 0)
                    {
                        _logger.LogInformation("✅ [WORKER CICLO] Se procesaron y enviaron {Total} recordatorios automáticos pendientes.", procesados);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [WORKER ERROR] Error al procesar recordatorios automáticos periódicos.");
            }

            await Task.Delay(_intervalo, stoppingToken);
        }

        _logger.LogInformation("🛑 [WORKER DETENIDO] Servicio de recordatorios finalizado.");
    }
}
