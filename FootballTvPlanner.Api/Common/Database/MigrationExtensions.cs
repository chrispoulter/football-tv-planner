using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Common.Database;

public static class MigrationExtensions
{
    public static async Task MigrateDatabaseAsync<TDbContext>(this IHost app)
        where TDbContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
