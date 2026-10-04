namespace FootballTvPlanner.Api.Data.Competitions;

public class Competition
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsHidden { get; set; }
}
