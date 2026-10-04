using FootballTvPlanner.Api.Data.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.Api.Data.Fixtures;

public class FixtureConfiguration : IEntityTypeConfiguration<Fixture>
{
    public void Configure(EntityTypeBuilder<Fixture> builder)
    {
        builder.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();

        builder.Property(f => f.Source).IsRequired();
        builder.Property(f => f.ExternalId).IsRequired();
        builder.Property(f => f.CompetitionId);
        builder.Property(f => f.HomeTeamId);
        builder.Property(f => f.AwayTeamId);
        builder.Property(f => f.KickoffUtc);

        builder.HasKey(f => f.Id);
        builder.HasIndex(f => new { f.Source, f.ExternalId }).IsUnique();
        builder.HasIndex(f => f.KickoffUtc);

        builder.HasQueryFilter(f => !f.Competition.IsHidden && f.Channels.Any(c => !c.IsHidden));

        builder
            .HasOne(f => f.Competition)
            .WithMany()
            .HasForeignKey(f => f.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(f => f.HomeTeam)
            .WithMany()
            .HasForeignKey(f => f.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(f => f.AwayTeam)
            .WithMany()
            .HasForeignKey(f => f.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(f => f.Channels)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "FixtureChannels",
                r =>
                    r.HasOne<Channel>()
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Restrict),
                l =>
                    l.HasOne<Fixture>()
                        .WithMany()
                        .HasForeignKey("FixtureId")
                        .OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("FixtureId", "ChannelId")
            );
    }
}
