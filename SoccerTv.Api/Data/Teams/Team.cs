namespace SoccerTv.Api.Data.Teams;

public class Team
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string? BadgeUrl { get; set; }
}
