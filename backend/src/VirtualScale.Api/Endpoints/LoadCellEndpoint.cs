using Microsoft.AspNetCore.Mvc;
using VirtualScale.Api.Dtos;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Endpoints;

public static class LoadCellEndpoint
{
    public static void MapLoadCellEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loadcells").WithTags("LoadCells");

        group
            .MapPost(
                "/factor",
                ([FromServices] Scale scale, [FromBody] LoadCellFactorRequest request) =>
                {
                    var updated = scale.SetLoadCellFactor(request.Id, request.Factor);
                    return updated
                        ? Results.Ok(new { status = "success" })
                        : Results.NotFound(new { status = "not_found" });
                }
            )
            .WithName("Set LoadCell Factor")
            .WithSummary("Set correction factor for a load cell by id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group
            .MapPost(
                "/factors/reset",
                ([FromServices] Scale scale) =>
                {
                    scale.ResetLoadCellFactors();
                    return Results.Ok(new { status = "success" });
                }
            )
            .WithName("Reset LoadCell Factors")
            .WithSummary("Reset correction factor of all load cells to 1.0")
            .Produces(StatusCodes.Status200OK);
    }
}
