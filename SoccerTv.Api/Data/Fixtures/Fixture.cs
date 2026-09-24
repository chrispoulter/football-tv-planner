using SoccerTv.Api.Data.Competitions;
using SoccerTv.Api.Data.Teams;

namespace SoccerTv.Api.Data.Fixtures;

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

    public string? Venue { get; set; }

    public FixtureStatus Status { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public List<FixtureBroadcast> Broadcasts { get; set; } = [];
}
