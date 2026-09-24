namespace SoccerTv.Api.Common.Fixtures;

public class FixtureProviderSettings
{
    public static string SectionName { get; } = "FixtureProvider";

    public string Provider { get; set; } = "Mock";

    public int SyncIntervalHours { get; set; } = 6;

    public int DaysAhead { get; set; } = 14;
}
