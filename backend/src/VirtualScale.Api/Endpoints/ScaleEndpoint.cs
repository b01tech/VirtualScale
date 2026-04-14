using Microsoft.AspNetCore.Mvc;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Endpoints;

public static class ScaleEndpoint
{
    public static void MapScaleEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/scale").WithTags("Scale");

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
