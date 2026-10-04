using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.FixtureSync.Providers.LiveFootballOnTv;

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
        CancellationToken cancellationToken = default
    )
    {
        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        var html = await httpClient.GetStringAsync(_settings.BaseUrl, cancellationToken);

        var document = await new HtmlParser().ParseDocumentAsync(html, cancellationToken);

        var fixtures = new List<ProviderFixture>();
        DateOnly? date = null;

        foreach (var element in document.QuerySelectorAll(".fixture-date, .fixture"))
        {
            if (element.ClassList.Contains("fixture-date"))
            {
                date = ParseDate(element.TextContent);
                continue;
            }

            if (date is not { } fixtureDate)
            {
                continue;
            }

            var fixture = ParseFixture(element, fixtureDate);

            if (fixture is not null)
            {
                fixtures.Add(fixture);
            }
        }

        return [.. fixtures.DistinctBy(f => f.ExternalId)];
    }

    private ProviderFixture? ParseFixture(IElement element, DateOnly date)
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

        var channels = element
            .QuerySelectorAll(".channel-pill")
            .Select(pill => pill.TextContent.Trim())
            .Distinct()
            .ToList();

        return new ProviderFixture(
            ExternalId: $"lfotv-{date:yyyyMMdd}-{Slug(home)}-{Slug(away)}",
            Competition: competitionText,
            HomeTeam: home,
            AwayTeam: away,
            KickoffUtc: DateTimeExtensions.ToUtc(date, time),
            Channels: channels
        );
    }

    private static DateOnly? ParseDate(string text)
    {
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

    private static string Text(IElement element, string selector) =>
        element.QuerySelector(selector)?.TextContent.Trim() ?? "";

    private static string Slug(string value) =>
        NonAlphanumeric().Replace(value.ToLowerInvariant(), "-").Trim('-');

    [GeneratedRegex(@"(\d+)(st|nd|rd|th)\b")]
    private static partial Regex OrdinalSuffix();

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphanumeric();
}
