namespace FootballTvPlanner.FixtureSync.Data;

public class Fixture
{
    public Guid Id { get; set; }

    public string Source { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public Guid CompetitionId { get; set; }

    public Competition Competition { get; set; } = null!;

    public Guid HomeTeamId { get; set; }

    public Team HomeTeam { get; set; } = null!;

    public Guid AwayTeamId { get; set; }

    public Team AwayTeam { get; set; } = null!;

    public DateTimeOffset KickoffUtc { get; set; }

    public List<Channel> Channels { get; set; } = [];
}
