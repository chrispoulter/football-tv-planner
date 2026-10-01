using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.FixtureSync.Data;

public class FixtureConfiguration : IEntityTypeConfiguration<Fixture>
{
    public void Configure(EntityTypeBuilder<Fixture> builder)
    {
        builder.ToTable(t => t.ExcludeFromMigrations());

        builder.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();

        builder.Property(f => f.Source).IsRequired();
        builder.Property(f => f.ExternalId).IsRequired();
        builder.Property(f => f.Competition).IsRequired();
        builder.Property(f => f.HomeTeam).IsRequired();
        builder.Property(f => f.AwayTeam).IsRequired();
        builder.Property(f => f.KickoffUtc);
        builder.Property(f => f.Channels).IsRequired();

        builder.HasKey(f => f.Id);
        builder.HasIndex(f => new { f.Source, f.ExternalId }).IsUnique();
    }
}
