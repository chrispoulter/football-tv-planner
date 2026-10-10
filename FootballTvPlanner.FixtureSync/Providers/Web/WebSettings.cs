namespace FootballTvPlanner.FixtureSync.Providers.Web;

public class WebSettings
{
    public static string SectionName { get; } = $"{FixtureSyncSettings.SectionName}:Web";

    public Uri Url { get; set; } = null!;
}
