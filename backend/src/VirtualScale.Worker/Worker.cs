using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualScale.Domain.Entities;
using VirtualScale.Worker.Services;

namespace VirtualScale.Worker;

public class Worker(ILogger<Worker> logger, Scale scale, SerialHandler serialHandler) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        serialHandler.StartReading();
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                scale.CalcWeight();
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation(scale.PrintData());
                }
                await Task.Delay(200, stoppingToken);
            }
        }
        finally
        {
            serialHandler.StopReading();
        }
    }
}
