using FootballTvPlanner.Data.Fixtures;
using FootballTvPlanner.Data.Users;

namespace FootballTvPlanner.Data.Schedule;

public class UserFixture
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid FixtureId { get; set; }

    public Fixture Fixture { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
