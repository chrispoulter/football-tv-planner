using System.Linq.Expressions;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Fixtures;

namespace FootballTvPlanner.Api.Features.Fixtures;

public record FixtureSummary(
    Guid Id,
    DateTimeOffset KickoffUtc,
    string Competition,
    string HomeTeam,
    string AwayTeam,
    List<string> Channels,
    bool IsBookmarked
);

public static class FixtureProjections
{
    public static Expression<Func<Fixture, FixtureSummary>> ToSummary(
        FootballTvPlannerDbContext dbContext,
        Guid? userId
    ) =>
        f => new FixtureSummary(
            f.Id,
            f.KickoffUtc,
            f.Competition,
            f.HomeTeam,
            f.AwayTeam,
            f.Channels,
            userId != null
                && dbContext.UserFixtures.Any(uf => uf.UserId == userId && uf.FixtureId == f.Id)
        );

    public static Expression<Func<Fixture, CalendarFixture>> ToCalendarFixture() =>
        f => new CalendarFixture(
            f.Id,
            f.KickoffUtc,
            f.HomeTeam,
            f.AwayTeam,
            f.Competition,
            f.Channels
        );
}
