using System.Linq.Expressions;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Fixtures;

namespace FootballTvPlanner.Api.Features.Fixtures.GetFixtures;

public record FixtureSummary(
    Guid Id,
    DateTimeOffset KickoffUtc,
    string Competition,
    string HomeTeam,
    string AwayTeam,
    List<string> Channels,
    bool IsBookmarked
)
{
    public static Expression<Func<Fixture, FixtureSummary>> FromFixture(
        FootballTvPlannerDbContext dbContext,
        Guid? userId
    ) =>
        f => new FixtureSummary(
            f.Id,
            f.KickoffUtc,
            f.Competition.DisplayName ?? f.Competition.Name,
            f.HomeTeam,
            f.AwayTeam,
            // Same channel rule as CalendarFixture.FromFixture.
            f.FixtureChannels.Where(fc => !fc.Channel.IsExcluded)
                .OrderBy(fc => fc.Channel.SortOrder == null)
                .ThenBy(fc => fc.Channel.SortOrder)
                .ThenBy(fc => fc.Channel.DisplayName ?? fc.Channel.Name)
                .Select(fc => fc.Channel.DisplayName ?? fc.Channel.Name)
                .ToList(),
            userId != null
                && dbContext.UserFixtures.Any(uf => uf.UserId == userId && uf.FixtureId == f.Id)
        );
}
