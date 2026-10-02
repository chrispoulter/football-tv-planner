namespace FootballTvPlanner.Api.Data.Competitions;

public class Competition
{
    public Guid Id { get; set; }

    /// <summary>
    /// The name as scraped from the provider. Set by FixtureSync and never edited.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Overrides <see cref="Name"/> in the UI when set.
    /// </summary>
    public string? DisplayName { get; set; }

    public bool IsExcluded { get; set; }

    /// <summary>
    /// Lower values come first. Unset values come after, ordered by name.
    /// </summary>
    public int? SortOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
