using FootballTvPlanner.Api.Data.Competitions;

namespace FootballTvPlanner.Api.Data.Fixtures;

public class Fixture
{
    public Guid Id { get; set; }

    public string Source { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public Guid CompetitionId { get; set; }

    public Competition Competition { get; set; } = null!;

    public string HomeTeam { get; set; } = null!;

    public string AwayTeam { get; set; } = null!;

    public DateTimeOffset KickoffUtc { get; set; }

    public ICollection<FixtureChannel> FixtureChannels { get; set; } = [];
}
