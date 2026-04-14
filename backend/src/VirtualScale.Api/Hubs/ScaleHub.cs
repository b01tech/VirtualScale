using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using VirtualScale.Api.Dtos;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Hubs;

public class ScaleHub(Scale scale) : Hub
{
    public ScaleResponse GetCurrent()
    {
        var (bruteWeight, netWeight, tareWeight) = scale.GetRoundedWeights();
        return new ScaleResponse(bruteWeight, netWeight, tareWeight, scale.IsTared);
    }

    public async IAsyncEnumerable<ScaleResponse> Stream([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return GetCurrent();
            await Task.Delay(200, cancellationToken);
        }
    }
}
