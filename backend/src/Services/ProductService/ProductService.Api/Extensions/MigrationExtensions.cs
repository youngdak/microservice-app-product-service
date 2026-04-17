using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure;

namespace ProductService.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task<WebApplication> ApplyMigrationCheks(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

            var retries = 5;

            while (retries > 0)
            {
                try
                {
                    var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        await db.Database.MigrateAsync();
                    }

                    return app;
                }
                catch
                {
                    retries--;
                    await Task.Delay(2000);
                }
            }

            if (retries == 0)
            {
                throw new Exception("Failed to apply migrations after retries.");
            }

            return app;
        }
    }
}
