namespace SoccerTv.FixtureSync;

public class FixtureSyncSettings
{
    public static string SectionName { get; } = "FixtureSync";

    public string Provider { get; set; } = "Mock";

    public int DaysAhead { get; set; } = 14;
}
