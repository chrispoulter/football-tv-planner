namespace FootballTvPlanner.FixtureSync.Providers.Mock;

public class MockSettings
{
    public static string SectionName { get; } = $"{FixtureSyncSettings.SectionName}:Mock";

    public int DaysAhead { get; set; } = 14;
}
