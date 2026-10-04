using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.Api.Data.Teams;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();

        builder.Property(t => t.Name).IsRequired();

        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Name).IsUnique();
    }
}
