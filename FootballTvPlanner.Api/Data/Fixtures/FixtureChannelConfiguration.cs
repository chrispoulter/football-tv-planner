using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.Api.Data.Fixtures;

public class FixtureChannelConfiguration : IEntityTypeConfiguration<FixtureChannel>
{
    public void Configure(EntityTypeBuilder<FixtureChannel> builder)
    {
        builder.Property(fc => fc.FixtureId);
        builder.Property(fc => fc.ChannelId);

        builder.HasKey(fc => new { fc.FixtureId, fc.ChannelId });

        builder
            .HasOne(fc => fc.Fixture)
            .WithMany(f => f.FixtureChannels)
            .HasForeignKey(fc => fc.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(fc => fc.Channel)
            .WithMany()
            .HasForeignKey(fc => fc.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
