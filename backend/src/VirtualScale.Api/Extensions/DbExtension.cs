using Microsoft.EntityFrameworkCore;
using VirtualScale.Api.Infrastructure.Persistence;
using VirtualScale.Api.Infrastructure.Services;
using VirtualScale.Domain.Entities;

namespace VirtualScale.Api.Extensions;

public static class DbExtension
{
    public static void AddDatabaseContext(this IServiceCollection services, IHostEnvironment environment)
    {
        var dbPath = Path.Combine(environment.ContentRootPath, "Data", "virtualscale.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        services.AddDbContext<CalibrationDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
    }

    public static async Task LoadDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CalibrationDbContext>();
        await context.Database.EnsureCreatedAsync();

        var persistenceService = scope.ServiceProvider.GetRequiredService<CalibrationPersistenceService>();
        var scale = scope.ServiceProvider.GetRequiredService<Scale>();
        await persistenceService.LoadCalibrationAsync(scale);
    }
}
