using System.Globalization;
using System.Text;

namespace SoccerTv.Api.Common.Calendar;

public record CalendarFixture(
    Guid Id,
    DateTimeOffset KickoffUtc,
    string HomeTeam,
    string AwayTeam,
    string Competition,
    IReadOnlyList<string> Channels
);

/// <summary>
/// Writes RFC 5545 iCalendar documents for fixtures. All times are written in UTC so no
/// VTIMEZONE block is needed; calendar clients convert to the viewer's local time.
/// </summary>
public static class CalendarBuilder
{
    public const string ContentType = "text/calendar; charset=utf-8";

    private const int ReminderMinutesBefore = 30;

    private static readonly TimeSpan MatchDuration = TimeSpan.FromHours(2);

    public static string Build(IEnumerable<CalendarFixture> fixtures, DateTimeOffset timestamp)
    {
        var sb = new StringBuilder();

        AppendLine(sb, "BEGIN:VCALENDAR");
        AppendLine(sb, "VERSION:2.0");
        AppendLine(sb, "PRODID:-//SoccerTv//Soccer TV Schedule//EN");
        AppendLine(sb, "CALSCALE:GREGORIAN");
        AppendLine(sb, "METHOD:PUBLISH");

        foreach (var fixture in fixtures)
        {
            AppendEvent(sb, fixture, timestamp);
        }

        AppendLine(sb, "END:VCALENDAR");

        return sb.ToString();
    }

    private static void AppendEvent(
        StringBuilder sb,
        CalendarFixture fixture,
        DateTimeOffset timestamp
    )
    {
        var summary = $"{fixture.HomeTeam} v {fixture.AwayTeam}";
        var channels = string.Join(", ", fixture.Channels);

        AppendLine(sb, "BEGIN:VEVENT");
        AppendLine(sb, $"UID:fixture-{fixture.Id}@soccertv");
        AppendLine(sb, $"DTSTAMP:{FormatUtc(timestamp)}");
        AppendLine(sb, $"DTSTART:{FormatUtc(fixture.KickoffUtc)}");
        AppendLine(sb, $"DTEND:{FormatUtc(fixture.KickoffUtc + MatchDuration)}");
        AppendLine(sb, $"SUMMARY:{Escape(summary)}");
        AppendLine(sb, $"DESCRIPTION:{Escape($"{fixture.Competition}\nWatch on: {channels}")}");
        AppendLine(sb, $"LOCATION:{Escape(channels)}");
        AppendLine(sb, $"CATEGORIES:{Escape(fixture.Competition)}");
        AppendLine(sb, "STATUS:CONFIRMED");
        AppendLine(sb, "TRANSP:TRANSPARENT");

        AppendLine(sb, "BEGIN:VALARM");
        AppendLine(sb, "ACTION:DISPLAY");
        AppendLine(sb, $"DESCRIPTION:{Escape($"{summary} on {channels}")}");
        AppendLine(sb, $"TRIGGER:-PT{ReminderMinutesBefore}M");
        AppendLine(sb, "END:VALARM");

        AppendLine(sb, "END:VEVENT");
    }

    private static string FormatUtc(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

    private static string Escape(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n");

    /// <summary>
    /// Content lines longer than 75 octets must be folded onto continuation lines that
    /// begin with a single space.
    /// </summary>
    private static void AppendLine(StringBuilder sb, string line)
    {
        const int maxOctets = 75;

        var octets = 0;

        foreach (var rune in line.EnumerateRunes())
        {
            var length = rune.Utf8SequenceLength;

            if (octets + length > maxOctets)
            {
                sb.Append("\r\n ");
                octets = 1;
            }

            sb.Append(rune.ToString());
            octets += length;
        }

        sb.Append("\r\n");
    }
}
