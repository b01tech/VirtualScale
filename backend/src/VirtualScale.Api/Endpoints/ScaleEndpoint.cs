using Microsoft.AspNetCore.Mvc;
using VirtualScale.Api.Dtos;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Endpoints;

public static class ScaleEndpoint
{
    public static void MapScaleEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/scale").WithTags("Scale");

        group
            .MapGet(
                "/",
                ([FromServices] Scale scale) =>
                {
                    var (bruteWeight, netWeight, tareWeight) = scale.GetRoundedWeights();
                    return new ScaleResponse(
                        bruteWeight,
                        netWeight,
                        tareWeight,
                        scale.IsTared,
                        scale.IsStable,
                        scale.FilterLevel,
                        scale.NumberOfCells,
                        scale.Calibration.CapMax,
                        scale.Calibration.Division,
                        scale.Calibration.DecimalPlaces,
                        scale.Calibration.ReferenceWeight,
                        scale.NeedsCalibrationAdjustment
                    );
                }
            )
            .WithName("Read Scale")
            .WithSummary("Read the current scale values")
            .Produces<ScaleResponse>(StatusCodes.Status200OK);

        group
            .MapPost(
                "/settings",
                ([FromServices] Scale scale, [FromBody] CalibrationSettingsRequest request) =>
                {
                    scale.UpdateCalibrationSettings(
                        request.NumberOfCells,
                        request.CapMax,
                        request.Division,
                        request.DecimalPlaces,
                        request.ReferenceWeight
                    );
                    return Results.Ok(new { status = "success" });
                }
            )
            .WithName("Set Calibration Settings")
            .WithSummary("Set calibration settings and mark calibration as required")
            .Produces(StatusCodes.Status200OK);

        group
            .MapPost(
                "/filter",
                ([FromServices] Scale scale, [FromBody] FilterLevelRequest request) =>
                {
                    scale.SetFilterLevel(request.Level);
                    return Results.Ok(new { status = "success" });
                }
            )
            .WithName("Set Filter Level")
            .WithSummary("Set digital filter level (0 disables)")
            .Produces(StatusCodes.Status200OK);

        group
            .MapPost(
                "/tare",
                ([FromServices] Scale scale) =>
                {
                    scale.Tare();
                    return new { status = "success" };
                }
            )
            .WithName("Tare Scale")
            .WithSummary("Tare the scale to the current weight")
            .Produces(StatusCodes.Status200OK);
        group
            .MapPost(
                "/calibrate/zero",
                ([FromServices] Scale scale) =>
                {
                    scale.CalibrateZero();
                    return new { status = "success" };
                }
            )
            .WithName("Calibrate Zero")
            .WithSummary("Take zero value")
            .Produces(StatusCodes.Status200OK);
        group
            .MapPost(
                "/calibrate/span",
                ([FromServices] Scale scale) =>
                {
                    scale.CalibrateSpan();
                    return new { status = "success" };
                }
            )
            .WithName("Calibrate Span")
            .WithSummary("Take span value")
            .Produces(StatusCodes.Status200OK);
    }
}
