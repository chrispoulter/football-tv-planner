using System.Linq.Expressions;
using SoccerTv.Api.Common.Calendar;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Channels;
using SoccerTv.Api.Data.Fixtures;

namespace SoccerTv.Api.Features.Fixtures;

public record FixtureSummary(
    Guid Id,
    DateTimeOffset KickoffUtc,
    FixtureStatus Status,
    CompetitionSummary Competition,
    TeamSummary HomeTeam,
    TeamSummary AwayTeam,
    List<ChannelSummary> Channels,
    bool IsBookmarked
);

public record CompetitionSummary(Guid Id, string Name, string ShortName);

public record TeamSummary(Guid Id, string Name, string ShortName, string? BadgeUrl);

public record ChannelSummary(Guid Id, string Name, string Provider, ChannelType Type);

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
            new CompetitionSummary(f.Competition.Id, f.Competition.Name, f.Competition.ShortName),
            new TeamSummary(
                f.HomeTeam.Id,
                f.HomeTeam.Name,
                f.HomeTeam.ShortName,
                f.HomeTeam.BadgeUrl
            ),
            new TeamSummary(
                f.AwayTeam.Id,
                f.AwayTeam.Name,
                f.AwayTeam.ShortName,
                f.AwayTeam.BadgeUrl
            ),
            f.Broadcasts.OrderBy(b => b.Channel.SortOrder)
                .Select(b => new ChannelSummary(
                    b.Channel.Id,
                    b.Channel.Name,
                    b.Channel.Provider,
                    b.Channel.Type
                ))
                .ToList(),
            userId != null
                && dbContext.UserFixtures.Any(uf => uf.UserId == userId && uf.FixtureId == f.Id)
        );

    public static Expression<Func<Fixture, CalendarFixture>> ToCalendarFixture() =>
        f => new CalendarFixture(
            f.Id,
            f.KickoffUtc,
            f.HomeTeam.Name,
            f.AwayTeam.Name,
            f.Competition.Name,
            f.Broadcasts.OrderBy(b => b.Channel.SortOrder).Select(b => b.Channel.Name).ToList(),
            f.Status == FixtureStatus.Cancelled
        );
}
