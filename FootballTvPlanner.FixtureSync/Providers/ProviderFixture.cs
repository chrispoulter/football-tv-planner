namespace FootballTvPlanner.FixtureSync.Providers;

public record ProviderFixture(
    string ExternalId,
    string Competition,
    string HomeTeam,
    string AwayTeam,
    DateTimeOffset KickoffUtc,
    IReadOnlyList<ProviderChannel> Channels
);

public record ProviderChannel(string Name, int SortOrder);
