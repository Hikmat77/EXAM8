using Application.Interfaces;

namespace WebApi.Services;

public class OrderExpirationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<OrderExpirationBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var expirationService = scope.ServiceProvider.GetRequiredService<IOrderExpirationService>();
                await expirationService.ExpirePendingOrdersAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to expire pending orders.");
            }
        }
    }
}
