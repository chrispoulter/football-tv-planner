using SoccerTv.Api.Data.Fixtures;

namespace SoccerTv.Api.Common.Fixtures;

public record ProviderFixture(
    string ExternalId,
    ProviderCompetition Competition,
    string HomeTeam,
    string AwayTeam,
    DateTimeOffset KickoffUtc,
    FixtureStatus Status,
    IReadOnlyList<ProviderChannel> Channels
);

public record ProviderCompetition(string Name, int SortOrder);

public record ProviderChannel(string Name, int SortOrder);
