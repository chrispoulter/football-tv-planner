using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballTvPlanner.Api.Data.Competitions;

public class CompetitionConfiguration : IEntityTypeConfiguration<Competition>
{
    public void Configure(EntityTypeBuilder<Competition> builder)
    {
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();

        builder.Property(c => c.Name).IsRequired();

        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
