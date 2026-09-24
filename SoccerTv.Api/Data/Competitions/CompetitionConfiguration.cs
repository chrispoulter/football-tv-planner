using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoccerTv.Api.Data.Competitions;

public class CompetitionConfiguration : IEntityTypeConfiguration<Competition>
{
    public void Configure(EntityTypeBuilder<Competition> builder)
    {
        builder.ToTable("competitions");

        builder
            .Property(c => c.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name).HasColumnName("name").IsRequired();
        builder.Property(c => c.ShortName).HasColumnName("short_name").IsRequired();
        builder.Property(c => c.Country).HasColumnName("country").IsRequired();
        builder.Property(c => c.SortOrder).HasColumnName("sort_order");

        builder.HasKey(c => c.Id).HasName("pk_competitions");
        builder.HasIndex(c => c.Name, "ix_competitions_name").IsUnique();
    }
}
