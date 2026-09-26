using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SoccerTv.Api.Data.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const int NameMaxLength = 100;

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name).HasMaxLength(NameMaxLength);

        builder.HasIndex(u => u.CalendarFeedToken).IsUnique();
    }
}
