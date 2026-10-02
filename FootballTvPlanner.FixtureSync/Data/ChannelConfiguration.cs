using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.FixtureSync.Data;

public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels", t => t.ExcludeFromMigrations());

        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(c => c.Name).IsRequired();

        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
