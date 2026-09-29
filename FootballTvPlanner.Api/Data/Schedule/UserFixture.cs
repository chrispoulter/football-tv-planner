using FootballTvPlanner.Api.Data.Fixtures;
using FootballTvPlanner.Api.Data.Users;

namespace FootballTvPlanner.Api.Data.Schedule;

public class UserFixture
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid FixtureId { get; set; }

    public Fixture Fixture { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
