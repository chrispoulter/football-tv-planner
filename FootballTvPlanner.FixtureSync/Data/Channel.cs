namespace FootballTvPlanner.FixtureSync.Data;

/// <summary>
/// Only the scraped name is mapped. Display name, exclusion and sort order are edited by hand
/// and the sync never touches them.
/// </summary>
public class Channel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}
