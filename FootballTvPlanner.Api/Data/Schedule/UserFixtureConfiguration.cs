using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.Api.Data.Schedule;

public class UserFixtureConfiguration : IEntityTypeConfiguration<UserFixture>
{
    public void Configure(EntityTypeBuilder<UserFixture> builder)
    {
        builder.Property(uf => uf.UserId);
        builder.Property(uf => uf.FixtureId);
        builder.Property(uf => uf.CreatedAt);

        builder.HasKey(uf => new { uf.UserId, uf.FixtureId });

        builder
            .HasOne(uf => uf.User)
            .WithMany()
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(uf => uf.Fixture)
            .WithMany()
            .HasForeignKey(uf => uf.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
