using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoccerTv.Api.Data.Channels;

public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("channels");

        builder
            .Property(c => c.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name).HasColumnName("name").IsRequired();
        builder.Property(c => c.Provider).HasColumnName("provider").IsRequired();
        builder.Property(c => c.Type).HasColumnName("type").HasConversion<string>().IsRequired();
        builder.Property(c => c.SortOrder).HasColumnName("sort_order");

        builder.HasKey(c => c.Id).HasName("pk_channels");
        builder.HasIndex(c => c.Name, "ix_channels_name").IsUnique();
    }
}
