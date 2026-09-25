using System.Linq.Expressions;
using SoccerTv.Api.Common.Calendar;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Fixtures;

namespace SoccerTv.Api.Features.Fixtures;

public record FixtureSummary(
    Guid Id,
    DateTimeOffset KickoffUtc,
    FixtureStatus Status,
    string Competition,
    string HomeTeam,
    string AwayTeam,
    List<ChannelSummary> Channels,
    bool IsBookmarked
);

public record ChannelSummary(string Name, string Provider, ChannelType Type);

public static class FixtureProjections
{
    public static Expression<Func<Fixture, FixtureSummary>> ToSummary(
        SoccerTvDbContext dbContext,
        Guid? userId
    ) =>
        f => new FixtureSummary(
            f.Id,
            f.KickoffUtc,
            f.Status,
            f.Competition,
            f.HomeTeam,
            f.AwayTeam,
            f.Channels.Select(c => new ChannelSummary(c.Name, c.Provider, c.Type)).ToList(),
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
            f.Channels.Select(c => c.Name).ToList(),
            f.Status == FixtureStatus.Cancelled
        );
}
