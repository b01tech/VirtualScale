namespace VirtualScale.Api.Endpoints;

public static class HealthEndpoint
{
    public static void MapHealthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", () => new { status = "healthy" })
            .WithTags("Health API Check")
            .WithSummary("Check the health of the API");
    }
}
