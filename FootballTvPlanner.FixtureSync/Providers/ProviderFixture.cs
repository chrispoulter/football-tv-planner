namespace FootballTvPlanner.FixtureSync.Providers;

public record ProviderFixture(
    string ExternalId,
    string Competition,
    string HomeTeam,
    string AwayTeam,
    DateTimeOffset KickoffUtc,
    IReadOnlyList<string> Channels
);
