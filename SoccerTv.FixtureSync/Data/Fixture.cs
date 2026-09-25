namespace SoccerTv.FixtureSync.Data;

/// <summary>
/// The sync's view of a row in the API's <c>fixtures</c> table. The API owns the schema
/// (see <c>SoccerTv.Api/Data/Fixtures</c>), so keep this in step with it.
/// </summary>
public class Fixture
{
    public Guid Id { get; set; }

    public string Source { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public string Competition { get; set; } = null!;

    public string HomeTeam { get; set; } = null!;

    public string AwayTeam { get; set; } = null!;

    public DateTimeOffset KickoffUtc { get; set; }

    /// <summary>
    /// Stored in display order.
    /// </summary>
    public List<string> Channels { get; set; } = [];
}
