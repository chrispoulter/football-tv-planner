using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.FixtureSync.Data;

public class FixtureChannelConfiguration : IEntityTypeConfiguration<FixtureChannel>
{
    public void Configure(EntityTypeBuilder<FixtureChannel> builder)
    {
        builder.ToTable("FixtureChannels", t => t.ExcludeFromMigrations());

        builder.Property(fc => fc.FixtureId);
        builder.Property(fc => fc.ChannelId);

        builder.HasKey(fc => new { fc.FixtureId, fc.ChannelId });

        builder
            .HasOne<Fixture>()
            .WithMany(f => f.FixtureChannels)
            .HasForeignKey(fc => fc.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Channel>().WithMany().HasForeignKey(fc => fc.ChannelId);
    }
}
