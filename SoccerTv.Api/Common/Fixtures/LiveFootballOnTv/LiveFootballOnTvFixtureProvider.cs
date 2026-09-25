using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Time;
using SoccerTv.Api.Data.Fixtures;

namespace SoccerTv.Api.Common.Fixtures.LiveFootballOnTv;

/// <summary>
/// Scrapes the UK televised football listings from live-footballontv.com. The home page
/// lists every upcoming fixture with its UK kick-off time and channels, so one request
/// covers the whole sync window.
/// </summary>
public partial class LiveFootballOnTvFixtureProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<LiveFootballOnTvSettings> settings,
    ILogger<LiveFootballOnTvFixtureProvider> logger
) : IFixtureProvider
{
    public const string HttpClientName = "LiveFootballOnTv";

    private readonly LiveFootballOnTvSettings _settings = settings.Value;

    public string Source => "LiveFootballOnTv";

    public async Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default
    )
    {
        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        var html = await httpClient.GetStringAsync(_settings.BaseUrl, cancellationToken);

        var document = await new HtmlParser().ParseDocumentAsync(html, cancellationToken);

        var fixtures = new List<ProviderFixture>();
        var unmatchedChannels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        DateOnly? date = null;

        // Dates and fixtures are siblings, so walk them in document order and carry the
        // most recent date forward.
        foreach (var element in document.QuerySelectorAll(".fixture-date, .fixture"))
        {
            if (element.ClassList.Contains("fixture-date"))
            {
                date = ParseDate(element.TextContent);
                continue;
            }

            if (date is not { } fixtureDate || fixtureDate < from || fixtureDate > to)
            {
                continue;
            }

            var fixture = ParseFixture(element, fixtureDate, unmatchedChannels);

            if (fixture is not null)
            {
                fixtures.Add(fixture);
            }
        }

        if (unmatchedChannels.Count > 0)
        {
            logger.LogDebug(
                "Ignored channels from {Source}: {Channels}",
                Source,
                string.Join(", ", unmatchedChannels.Order())
            );
        }

        return [.. fixtures.DistinctBy(f => f.ExternalId)];
    }

    private ProviderFixture? ParseFixture(
        IElement element,
        DateOnly date,
        HashSet<string> unmatchedChannels
    )
    {
        var timeText = Text(element, ".fixture__time");
        var teamsText = Text(element, ".fixture__teams");
        var competitionText = Text(element, ".fixture__competition");

        if (
            !TimeOnly.TryParseExact(
                timeText,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var time
            )
        )
        {
            logger.LogDebug(
                "Skipping {Teams} on {Date}: kick-off '{Time}'",
                teamsText,
                date,
                timeText
            );
            return null;
        }

        if (teamsText.Split(" v ", 2, StringSplitOptions.TrimEntries) is not [var home, var away])
        {
            logger.LogDebug("Skipping unrecognised fixture '{Teams}' on {Date}", teamsText, date);
            return null;
        }

        var channels = new List<ProviderChannel>();

        foreach (var pill in element.QuerySelectorAll(".channel-pill"))
        {
            var name = pill.TextContent.Trim();
            var channel = MatchChannel(name);

            if (channel is null)
            {
                unmatchedChannels.Add(name);
            }
            else if (!channels.Any(c => c.Name == channel.Name))
            {
                channels.Add(channel);
            }
        }

        if (channels.Count == 0)
        {
            return null;
        }

        return new ProviderFixture(
            ExternalId: $"lfotv-{date:yyyyMMdd}-{Slug(home)}-{Slug(away)}",
            Competition: GetCompetition(competitionText),
            HomeTeam: home,
            AwayTeam: away,
            KickoffUtc: UkTime.ToUtc(date, time),
            Channels: channels
        );
    }

    private ProviderChannel? MatchChannel(string name)
    {
        var rule = _settings.Channels.FirstOrDefault(r => StartsWord(name, r.Match));

        if (rule is null)
        {
            return null;
        }

        var isStreaming = _settings.StreamingKeywords.Any(k =>
            name.Contains(k, StringComparison.OrdinalIgnoreCase)
        );

        // Keep a broadcaster's channels together, with TV ahead of its streaming services.
        return isStreaming
            ? new ProviderChannel(name, rule.SortOrder + 1)
            : new ProviderChannel(name, rule.SortOrder);
    }

    private static string GetCompetition(string text)
    {
        // The competition is followed by the stage (e.g. "Group Stage") after a
        // non-breaking space.
        return text.Split(' ', 2)[0].Trim();
    }

    private static DateOnly? ParseDate(string text)
    {
        // "Friday 25th September 2026"
        var normalised = OrdinalSuffix().Replace(text.Trim(), "$1");

        return DateOnly.TryParseExact(
            normalised,
            "dddd d MMMM yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date
        )
            ? date
            : null;
    }

    /// <summary>
    /// Whether <paramref name="keyword"/> appears at the start of a word, so "ITV" matches
    /// "ITV1" and "ITVX" but not "LOITV".
    /// </summary>
    private static bool StartsWord(string name, string keyword) =>
        Regex.IsMatch(
            name,
            $@"(?<![\p{{L}}\p{{N}}]){Regex.Escape(keyword)}",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
        );

    private static string Text(IElement element, string selector) =>
        element.QuerySelector(selector)?.TextContent.Trim() ?? "";

    private static string Slug(string value) =>
        NonAlphanumeric().Replace(value.ToLowerInvariant(), "-").Trim('-');

    [GeneratedRegex(@"(\d+)(st|nd|rd|th)\b")]
    private static partial Regex OrdinalSuffix();

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphanumeric();
}
