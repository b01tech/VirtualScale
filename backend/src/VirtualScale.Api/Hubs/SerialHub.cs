using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using VirtualScale.Api.Dtos;
using VirtualScale.Worker.Services;

namespace VirtualScale.Api.Hubs;

public class SerialHub(SerialHandler serialHandler) : Hub
{
    public SerialStatusResponse GetCurrent()
    {
        return new SerialStatusResponse(
            serialHandler.DesiredConnected,
            serialHandler.DesiredPort,
            serialHandler.IsConnected,
            serialHandler.ConnectedPort,
            serialHandler.State,
            serialHandler.LastError
        );
    }

    public async IAsyncEnumerable<SerialStatusResponse> Stream(
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return GetCurrent();
            await Task.Delay(500, cancellationToken);
        }
    }
}
