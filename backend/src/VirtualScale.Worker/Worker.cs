using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Worker;

public class Worker(ILogger<Worker> logger, Scale scale) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            scale.CalcWeight();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(scale.PrintData());
            }
            await Task.Delay(100, stoppingToken);
        }
    }
}
