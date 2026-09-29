using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.Api.Data.Schedule;

public class UserFixtureConfiguration : IEntityTypeConfiguration<UserFixture>
{
    public void Configure(EntityTypeBuilder<UserFixture> builder)
    {
        builder.ToTable("user_fixtures");

        builder.Property(uf => uf.UserId).HasColumnName("user_id");
        builder.Property(uf => uf.FixtureId).HasColumnName("fixture_id");
        builder.Property(uf => uf.CreatedAt).HasColumnName("created_at");

        builder.HasKey(uf => new { uf.UserId, uf.FixtureId }).HasName("pk_user_fixtures");

        builder
            .HasOne(uf => uf.User)
            .WithMany()
            .HasForeignKey(uf => uf.UserId)
            .HasConstraintName("fk_user_fixtures_users_user_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(uf => uf.Fixture)
            .WithMany()
            .HasForeignKey(uf => uf.FixtureId)
            .HasConstraintName("fk_user_fixtures_fixtures_fixture_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
