using System.Linq.Expressions;
using FootballTvPlanner.Api.Data.Fixtures;

namespace FootballTvPlanner.Api.Features.Fixtures;

public static class FixtureProjections
{
    public static Expression<Func<Fixture, CalendarFixture>> ToCalendarFixture() =>
        f => new CalendarFixture(
            f.Id,
            f.KickoffUtc,
            f.HomeTeam.Name,
            f.AwayTeam.Name,
            f.Competition.Name,
            f.Channels.OrderBy(c => c.Name).Select(c => c.Name).ToList()
        );
}
