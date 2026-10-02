namespace FootballTvPlanner.FixtureSync.Data;

public class Fixture
{
    public Guid Id { get; set; }

    public string Source { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public Guid CompetitionId { get; set; }

    public string HomeTeam { get; set; } = null!;

    public string AwayTeam { get; set; } = null!;

    public DateTimeOffset KickoffUtc { get; set; }

    public List<FixtureChannel> FixtureChannels { get; set; } = [];
}
