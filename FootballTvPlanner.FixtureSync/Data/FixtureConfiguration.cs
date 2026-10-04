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
        builder.Property(f => f.CompetitionId);
        builder.Property(f => f.HomeTeamId);
        builder.Property(f => f.AwayTeamId);
        builder.Property(f => f.KickoffUtc);

        builder.HasKey(f => f.Id);
        builder.HasIndex(f => new { f.Source, f.ExternalId }).IsUnique();

        builder.HasOne(f => f.Competition).WithMany().HasForeignKey(f => f.CompetitionId);
        builder.HasOne(f => f.HomeTeam).WithMany().HasForeignKey(f => f.HomeTeamId);
        builder.HasOne(f => f.AwayTeam).WithMany().HasForeignKey(f => f.AwayTeamId);

        builder
            .HasMany(f => f.Channels)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "FixtureChannels",
                r => r.HasOne<Channel>().WithMany().HasForeignKey("ChannelId"),
                l => l.HasOne<Fixture>().WithMany().HasForeignKey("FixtureId"),
                j =>
                {
                    j.ToTable(t => t.ExcludeFromMigrations());
                    j.HasKey("FixtureId", "ChannelId");
                }
            );
    }
}
