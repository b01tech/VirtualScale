using VirtualScale.Api.Hubs;

namespace VirtualScale.Api.Extensions;

public static class HubExtension
{
    public static void MapAppHubs(this WebApplication app)
    {
        app.MapHub<ScaleHub>("/hubs/scale");
        app.MapHub<LoadCellHub>("/hubs/loadcells");
        app.MapHub<SerialHub>("/hubs/serial");
    }
}
