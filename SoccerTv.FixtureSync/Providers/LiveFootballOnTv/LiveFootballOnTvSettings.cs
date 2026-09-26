namespace SoccerTv.FixtureSync.Providers.LiveFootballOnTv;

public class LiveFootballOnTvSettings
{
    public static string SectionName { get; } =
        $"{FixtureSyncSettings.SectionName}:LiveFootballOnTv";

    public Uri BaseUrl { get; set; } = new("https://www.live-footballontv.com/");
}
