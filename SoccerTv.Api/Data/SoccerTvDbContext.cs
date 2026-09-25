using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Data.Fixtures;
using SoccerTv.Api.Data.Schedule;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Data;

public class SoccerTvDbContext(DbContextOptions<SoccerTvDbContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Fixture> Fixtures { get; set; }

    public virtual DbSet<UserFixture> UserFixtures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
