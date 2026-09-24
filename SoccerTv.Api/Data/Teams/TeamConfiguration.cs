using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoccerTv.Api.Data.Teams;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams");

        builder
            .Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Name).HasColumnName("name").IsRequired();
        builder.Property(t => t.ShortName).HasColumnName("short_name").IsRequired();
        builder.Property(t => t.BadgeUrl).HasColumnName("badge_url");

        builder.HasKey(t => t.Id).HasName("pk_teams");
        builder.HasIndex(t => t.Name, "ix_teams_name").IsUnique();
    }
}
