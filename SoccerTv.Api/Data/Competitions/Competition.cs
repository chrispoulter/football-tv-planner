namespace SoccerTv.Api.Data.Competitions;

public class Competition
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int SortOrder { get; set; }
}
