using Microsoft.AspNetCore.Mvc;
using VirtualScale.Api.Dtos;
using VirtualScale.Worker.Services;

namespace VirtualScale.Api.Endpoints;

public static class SerialEndpoint
{
    public static void MapSerialEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/serial").WithTags("Serial");

        group
            .MapGet("/ports", ([FromServices] SerialHandler serialHandler) => serialHandler.GetAvailablePorts())
            .WithName("Serial Ports")
            .WithSummary("List available serial ports")
            .Produces<string[]>(StatusCodes.Status200OK);

        group
            .MapGet(
                "/status",
                ([FromServices] SerialHandler serialHandler) =>
                    new SerialStatusResponse(
                        serialHandler.DesiredConnected,
                        serialHandler.DesiredPort,
                        serialHandler.IsConnected,
                        serialHandler.ConnectedPort,
                        serialHandler.State,
                        serialHandler.LastError
                    )
            )
            .WithName("Serial Status")
            .WithSummary("Get current serial connection status")
            .Produces<SerialStatusResponse>(StatusCodes.Status200OK);

        group
            .MapPost(
                "/connect",
                ([FromServices] SerialHandler serialHandler, [FromBody] SerialConnectRequest request) =>
                {
                    serialHandler.RequestConnect(request.Port);
                    serialHandler.EnsureConnected();
                    return Results.Ok(
                        new SerialStatusResponse(
                            serialHandler.DesiredConnected,
                            serialHandler.DesiredPort,
                            serialHandler.IsConnected,
                            serialHandler.ConnectedPort,
                            serialHandler.State,
                            serialHandler.LastError
                        )
                    );
                }
            )
            .WithName("Serial Connect")
            .WithSummary("Request serial connection to a port")
            .Produces<SerialStatusResponse>(StatusCodes.Status200OK);

        group
            .MapPost(
                "/disconnect",
                ([FromServices] SerialHandler serialHandler) =>
                {
                    serialHandler.RequestDisconnect();
                    return Results.Ok(
                        new SerialStatusResponse(
                            serialHandler.DesiredConnected,
                            serialHandler.DesiredPort,
                            serialHandler.IsConnected,
                            serialHandler.ConnectedPort,
                            serialHandler.State,
                            serialHandler.LastError
                        )
                    );
                }
            )
            .WithName("Serial Disconnect")
            .WithSummary("Disconnect serial port")
            .Produces<SerialStatusResponse>(StatusCodes.Status200OK);
    }
}
