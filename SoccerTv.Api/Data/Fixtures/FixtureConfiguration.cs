using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoccerTv.Api.Data.Fixtures;

public class FixtureConfiguration : IEntityTypeConfiguration<Fixture>
{
    public void Configure(EntityTypeBuilder<Fixture> builder)
    {
        builder.ToTable("fixtures");

        builder
            .Property(f => f.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Source).HasColumnName("source").IsRequired();
        builder.Property(f => f.ExternalId).HasColumnName("external_id").IsRequired();
        builder.Property(f => f.CompetitionId).HasColumnName("competition_id");
        builder.Property(f => f.HomeTeamId).HasColumnName("home_team_id");
        builder.Property(f => f.AwayTeamId).HasColumnName("away_team_id");
        builder.Property(f => f.KickoffUtc).HasColumnName("kickoff_utc");
        builder.Property(f => f.Venue).HasColumnName("venue");
        builder
            .Property(f => f.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");

        builder.HasKey(f => f.Id).HasName("pk_fixtures");
        builder
            .HasIndex(f => new { f.Source, f.ExternalId }, "ix_fixtures_source_external_id")
            .IsUnique();
        builder.HasIndex(f => f.KickoffUtc, "ix_fixtures_kickoff_utc");

        builder
            .HasOne(f => f.Competition)
            .WithMany()
            .HasForeignKey(f => f.CompetitionId)
            .HasConstraintName("fk_fixtures_competitions_competition_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(f => f.HomeTeam)
            .WithMany()
            .HasForeignKey(f => f.HomeTeamId)
            .HasConstraintName("fk_fixtures_teams_home_team_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(f => f.AwayTeam)
            .WithMany()
            .HasForeignKey(f => f.AwayTeamId)
            .HasConstraintName("fk_fixtures_teams_away_team_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
