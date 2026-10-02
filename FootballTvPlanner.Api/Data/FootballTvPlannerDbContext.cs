using System.Reflection;
using FootballTvPlanner.Api.Data.Channels;
using FootballTvPlanner.Api.Data.Competitions;
using FootballTvPlanner.Api.Data.Fixtures;
using FootballTvPlanner.Api.Data.Schedule;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Data;

public class FootballTvPlannerDbContext(DbContextOptions<FootballTvPlannerDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options),
        IDataProtectionKeyContext
{
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Fixture> Fixtures { get; set; }

    public virtual DbSet<FixtureChannel> FixtureChannels { get; set; }

    public virtual DbSet<UserFixture> UserFixtures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
