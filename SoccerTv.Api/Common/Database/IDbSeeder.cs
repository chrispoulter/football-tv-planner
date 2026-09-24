using Microsoft.EntityFrameworkCore;

namespace SoccerTv.Api.Common.Database;

public interface IDbSeeder<in TDbContext>
    where TDbContext : DbContext
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
