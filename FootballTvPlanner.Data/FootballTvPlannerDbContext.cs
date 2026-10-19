using System.Reflection;
using FootballTvPlanner.Data.Channels;
using FootballTvPlanner.Data.Competitions;
using FootballTvPlanner.Data.Fixtures;
using FootballTvPlanner.Data.Schedule;
using FootballTvPlanner.Data.Teams;
using FootballTvPlanner.Data.Users;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Data;

public class FootballTvPlannerDbContext(DbContextOptions<FootballTvPlannerDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options),
        IDataProtectionKeyContext
{
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Fixture> Fixtures { get; set; }

    public virtual DbSet<UserFixture> UserFixtures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
