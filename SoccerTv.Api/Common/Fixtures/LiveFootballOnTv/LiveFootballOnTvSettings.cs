namespace SoccerTv.Api.Common.Fixtures.LiveFootballOnTv;

public class LiveFootballOnTvSettings
{
    public static string SectionName { get; } =
        $"{FixtureProviderSettings.SectionName}:LiveFootballOnTv";

    public Uri BaseUrl { get; set; } = new("https://www.live-footballontv.com/");

    /// <summary>
    /// Channels are kept when a word in their name starts with a rule's
    /// <see cref="ChannelRule.Match"/> text (case-insensitive). The first matching rule wins. Fixtures with no matching channels are skipped.
    /// </summary>
    public List<ChannelRule> Channels { get; set; } = [];

    /// <summary>
    /// A kept channel is treated as streaming when its name contains any of these, and is
    /// listed after the same broadcaster's TV channels.
    /// </summary>
    public List<string> StreamingKeywords { get; set; } = [];

    /// <summary>
    /// Optional overrides keyed by the competition name as it appears on the site.
    /// Competitions not listed here are still ingested with default values.
    /// </summary>
    public Dictionary<string, CompetitionOverride> Competitions { get; set; } = [];
}

public class ChannelRule
{
    public string Match { get; set; } = "";

    public int SortOrder { get; set; }
}

public class CompetitionOverride
{
    public int? SortOrder { get; set; }
}
