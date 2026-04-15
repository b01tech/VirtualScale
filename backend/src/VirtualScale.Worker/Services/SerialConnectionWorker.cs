using Microsoft.Extensions.Hosting;

namespace VirtualScale.Worker.Services;

public class SerialConnectionWorker(SerialHandler serialHandler) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            serialHandler.EnsureConnected();
            await Task.Delay(1000, stoppingToken);
        }
    }
}

