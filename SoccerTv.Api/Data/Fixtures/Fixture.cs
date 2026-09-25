namespace SoccerTv.Api.Data.Fixtures;

public class Fixture
{
    public Guid Id { get; set; }

    public string Source { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public string Competition { get; set; } = null!;

    public int CompetitionSortOrder { get; set; }

    public string HomeTeam { get; set; } = null!;

    public string AwayTeam { get; set; } = null!;

    public DateTimeOffset KickoffUtc { get; set; }

    public FixtureStatus Status { get; set; }

    /// <summary>
    /// Stored in display order.
    /// </summary>
    public List<string> Channels { get; set; } = [];
}
