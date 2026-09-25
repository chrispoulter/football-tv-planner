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
        builder.Property(f => f.Competition).HasColumnName("competition").IsRequired();
        builder.Property(f => f.CompetitionSortOrder).HasColumnName("competition_sort_order");
        builder.Property(f => f.HomeTeam).HasColumnName("home_team").IsRequired();
        builder.Property(f => f.AwayTeam).HasColumnName("away_team").IsRequired();
        builder.Property(f => f.KickoffUtc).HasColumnName("kickoff_utc");
        builder
            .Property(f => f.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsMany(
            f => f.Channels,
            channels =>
            {
                channels.ToJson("channels");
                channels.Property(c => c.Name).HasJsonPropertyName("name");
                channels.Property(c => c.Provider).HasJsonPropertyName("provider");
                channels.Property(c => c.Type).HasJsonPropertyName("type").HasConversion<string>();
            }
        );

        builder.HasKey(f => f.Id).HasName("pk_fixtures");
        builder
            .HasIndex(f => new { f.Source, f.ExternalId }, "ix_fixtures_source_external_id")
            .IsUnique();
        builder.HasIndex(f => f.KickoffUtc, "ix_fixtures_kickoff_utc");
        builder.HasIndex(f => f.Competition, "ix_fixtures_competition");
    }
}
