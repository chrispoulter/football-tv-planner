using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Data.Users;

public class User : IdentityUser<Guid>
{
    public string? Name { get; set; }

    public string? CalendarFeedToken { get; set; }
}
