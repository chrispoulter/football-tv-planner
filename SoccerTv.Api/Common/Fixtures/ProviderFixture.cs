using SoccerTv.Api.Data.Channels;
using SoccerTv.Api.Data.Fixtures;

namespace SoccerTv.Api.Common.Fixtures;

public record ProviderFixture(
    string ExternalId,
    ProviderCompetition Competition,
    ProviderTeam HomeTeam,
    ProviderTeam AwayTeam,
    DateTimeOffset KickoffUtc,
    string? Venue,
    FixtureStatus Status,
    IReadOnlyList<ProviderChannel> Channels
);

public record ProviderCompetition(string Name, string ShortName, string Country, int SortOrder);

public record ProviderTeam(string Name, string ShortName);

public record ProviderChannel(string Name, string Provider, ChannelType Type, int SortOrder);
