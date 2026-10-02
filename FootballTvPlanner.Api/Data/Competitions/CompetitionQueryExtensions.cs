namespace FootballTvPlanner.Api.Data.Competitions;

public static class CompetitionQueryExtensions
{
    /// <summary>
    /// Custom sort order first, then the rest alphabetically by display name.
    /// </summary>
    public static IOrderedQueryable<Competition> InDisplayOrder(
        this IQueryable<Competition> query
    ) =>
        query
            .OrderBy(c => c.SortOrder == null)
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.DisplayName ?? c.Name);
}
