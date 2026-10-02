using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.FixtureSync.Data;

public class FixtureSyncDbContext(DbContextOptions<FixtureSyncDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Fixture> Fixtures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
