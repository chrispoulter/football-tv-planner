namespace FootballTvPlanner.Api.Data.Channels;

public static class ChannelQueryExtensions
{
    /// <summary>
    /// Custom sort order first, then the rest alphabetically by display name.
    /// </summary>
    public static IOrderedQueryable<Channel> InDisplayOrder(this IQueryable<Channel> query) =>
        query
            .OrderBy(c => c.SortOrder == null)
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.DisplayName ?? c.Name);
}
