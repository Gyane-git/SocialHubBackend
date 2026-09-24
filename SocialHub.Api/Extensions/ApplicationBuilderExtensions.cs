using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;

namespace SocialHub.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<SocialHubDbContext>>();

        try
        {
            var context = services.GetRequiredService<SocialHubDbContext>();
            
            logger.LogInformation("Applying EF Core migrations if pending...");
            await context.Database.MigrateAsync();
            logger.LogInformation("EF Core migrations applied successfully.");

            logger.LogInformation("Seeding database...");
            await DbInitializer.SeedAsync(context);
            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}
