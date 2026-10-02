using System.Linq.Expressions;
using FootballTvPlanner.Api.Data.Fixtures;

namespace FootballTvPlanner.Api.Features.Fixtures;

public record CalendarFixture(
    Guid Id,
    DateTimeOffset KickoffUtc,
    string HomeTeam,
    string AwayTeam,
    string Competition,
    IReadOnlyList<string> Channels
)
{
    public static Expression<Func<Fixture, CalendarFixture>> FromFixture() =>
        f => new CalendarFixture(
            f.Id,
            f.KickoffUtc,
            f.HomeTeam,
            f.AwayTeam,
            f.Competition.DisplayName ?? f.Competition.Name,
            // Same channel rule as FixtureSummary.FromFixture.
            f.FixtureChannels.Where(fc => !fc.Channel.IsExcluded)
                .OrderBy(fc => fc.Channel.SortOrder == null)
                .ThenBy(fc => fc.Channel.SortOrder)
                .ThenBy(fc => fc.Channel.DisplayName ?? fc.Channel.Name)
                .Select(fc => fc.Channel.DisplayName ?? fc.Channel.Name)
                .ToList()
        );
}
