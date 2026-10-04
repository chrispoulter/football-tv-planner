namespace FootballTvPlanner.Api.Features.Fixtures.GetFixtures;

public record GetFixturesResponse(List<GetFixturesItem> Items);

public record GetFixturesItem(
    Guid Id,
    DateTimeOffset KickoffUtc,
    GetFixturesCompetition Competition,
    GetFixturesTeam HomeTeam,
    GetFixturesTeam AwayTeam,
    List<GetFixturesChannel> Channels,
    bool IsBookmarked
);

public record GetFixturesCompetition(Guid Id, string Name);

public record GetFixturesTeam(Guid Id, string Name);

public record GetFixturesChannel(Guid Id, string Name);
