namespace VirtualScale.Api.Extensions;

public static class CorsExtension
{
    public const string CorsPolicyName = "frontend";

    public static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                name: CorsPolicyName,
                builder =>
                {
                    builder.WithOrigins("http://localhost:1420").AllowAnyMethod().AllowAnyHeader();
                }
            );
        });
    }

    public static void UseCorsPolicy(this WebApplication app)
    {
        app.UseCors(CorsPolicyName);
    }
}
