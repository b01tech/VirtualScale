using VirtualScale.Worker.Services;

namespace VirtualScale.Api.Dtos;

public record SerialStatusResponse(
    bool DesiredConnected,
    string? DesiredPort,
    bool IsConnected,
    string? ConnectedPort,
    SerialConnectionState State,
    string? LastError
);
