using VirtualScale.Api.Endpoints;

namespace VirtualScale.Api.Extensions;

public static class EndpointExtension
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthEndpoint();
        app.MapScaleEndpoint();
    }
}
