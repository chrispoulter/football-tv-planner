using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SoccerTv.FixtureSync.Data;

public class FixtureSyncDbContext(DbContextOptions<FixtureSyncDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<Fixture> Fixtures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
