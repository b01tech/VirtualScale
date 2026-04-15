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
        var unitFactor = scale.Calibration.Unit == "g" ? 1000.0m : 1.0m;
        return new ScaleResponse(
            bruteWeight,
            netWeight,
            tareWeight,
            scale.IsTared,
            scale.IsOnZero,
            scale.IsStable,
            scale.FilterLevel,
            scale.NumberOfCells,
            scale.Calibration.CapMax * unitFactor,
            scale.Calibration.Division,
            scale.Calibration.DecimalPlaces,
            scale.Calibration.ReferenceWeight * unitFactor,
            (decimal)scale.Calibration.Resolution * unitFactor,
            scale.Calibration.Unit,
            scale.NeedsCalibrationAdjustment
        );
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
