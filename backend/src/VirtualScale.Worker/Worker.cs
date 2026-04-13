using VirtualScale.Domain.Entities;

namespace VirtualScale.Worker;

public class Worker(ILogger<Worker> logger, Scale scale) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(scale.PrintData());
                logger.LogInformation("-----------------");
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
