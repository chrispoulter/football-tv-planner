namespace FootballTvPlanner.Api.Data.Fixtures;

public static class FixtureQueryExtensions
{
    /// <summary>
    /// Fixtures in a competition that isn't excluded, showing on at least one channel that isn't.
    /// </summary>
    public static IQueryable<Fixture> Visible(this IQueryable<Fixture> query) =>
        query.Where(f =>
            !f.Competition.IsExcluded && f.FixtureChannels.Any(fc => !fc.Channel.IsExcluded)
        );

    /// <summary>
    /// Fixtures grouped by competition in display order, then by kick-off.
    /// </summary>
    public static IOrderedQueryable<Fixture> InDisplayOrder(this IQueryable<Fixture> query) =>
        query
            .OrderBy(f => f.Competition.SortOrder == null)
            .ThenBy(f => f.Competition.SortOrder)
            .ThenBy(f => f.Competition.DisplayName ?? f.Competition.Name)
            .ThenBy(f => f.KickoffUtc);
}
