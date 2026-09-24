using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoccerTv.Api.Data.Fixtures;

public class FixtureBroadcastConfiguration : IEntityTypeConfiguration<FixtureBroadcast>
{
    public void Configure(EntityTypeBuilder<FixtureBroadcast> builder)
    {
        builder.ToTable("fixture_broadcasts");

        builder.Property(b => b.FixtureId).HasColumnName("fixture_id");
        builder.Property(b => b.ChannelId).HasColumnName("channel_id");

        builder.HasKey(b => new { b.FixtureId, b.ChannelId }).HasName("pk_fixture_broadcasts");

        builder
            .HasOne(b => b.Fixture)
            .WithMany(f => f.Broadcasts)
            .HasForeignKey(b => b.FixtureId)
            .HasConstraintName("fk_fixture_broadcasts_fixtures_fixture_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(b => b.Channel)
            .WithMany()
            .HasForeignKey(b => b.ChannelId)
            .HasConstraintName("fk_fixture_broadcasts_channels_channel_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
