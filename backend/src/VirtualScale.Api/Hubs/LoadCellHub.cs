using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using VirtualScale.Api.Dtos;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Hubs;

public class LoadCellHub(Scale scale) : Hub
{
    public IEnumerable<LoadCellResponse> GetCurrent()
    {
        return scale.LoadCells.Select(cell => new LoadCellResponse(cell.Id, cell.RawValue, cell.Factor, cell.Status));
    }

    public async IAsyncEnumerable<LoadCellResponse> Stream(
        int? id,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var cells = scale.LoadCells;

            foreach (var cell in cells)
            {
                if (id is not null && cell.Id != id.Value)
                {
                    continue;
                }

                yield return new LoadCellResponse(cell.Id, cell.RawValue, cell.Factor, cell.Status);
            }

            await Task.Delay(200, cancellationToken);
        }
    }
}
