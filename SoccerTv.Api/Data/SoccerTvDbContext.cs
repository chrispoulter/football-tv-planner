using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Data.Fixtures;
using SoccerTv.Api.Data.Schedule;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Data;

public class SoccerTvDbContext(DbContextOptions<SoccerTvDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public virtual DbSet<Fixture> Fixtures { get; set; }

    public virtual DbSet<UserFixture> UserFixtures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
