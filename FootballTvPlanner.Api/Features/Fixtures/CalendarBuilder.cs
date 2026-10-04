using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace FootballTvPlanner.Api.Features.Fixtures;

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

    public static string Build(
        IEnumerable<CalendarFixture> fixtures,
        DateTimeOffset timestamp,
        string? calendarName = null
    )
    {
        var calendar = new Calendar
        {
            ProductId = "-//FootballTvPlanner//Football TV Planner//EN",
            Method = CalendarMethods.Publish,
        };

        if (calendarName is not null)
        {
            calendar.AddProperty("X-WR-CALNAME", calendarName);
            calendar.AddProperty(DurationProperty("REFRESH-INTERVAL", "PT1H"));
            calendar.AddProperty("X-PUBLISHED-TTL", "PT1H");
        }

        foreach (var fixture in fixtures)
        {
            calendar.Events.Add(CreateEvent(fixture, timestamp));
        }

        return new CalendarSerializer().SerializeToString(calendar)!;
    }

    private static CalendarEvent CreateEvent(CalendarFixture fixture, DateTimeOffset timestamp)
    {
        var summary = $"{fixture.HomeTeam} v {fixture.AwayTeam}";
        var channels = string.Join(", ", fixture.Channels);

        var calendarEvent = new CalendarEvent
        {
            Uid = $"fixture-{fixture.Id}@footballtvplanner",
            DtStamp = ToUtc(timestamp),
            DtStart = ToUtc(fixture.KickoffUtc),
            DtEnd = ToUtc(fixture.KickoffUtc + MatchDuration),
            Summary = summary,
            Description = $"{fixture.Competition}\nWatch on: {channels}",
            Location = channels,
            Categories = [fixture.Competition],
            Status = EventStatus.Confirmed,
            Transparency = TransparencyType.Transparent,
        };

        calendarEvent.Alarms.Add(
            new Alarm
            {
                Action = AlarmAction.Display,
                Description = $"{summary} on {channels}",
                Trigger = new Trigger(Duration.FromMinutes(-ReminderMinutesBefore)),
            }
        );

        return calendarEvent;
    }

    private static CalDateTime ToUtc(DateTimeOffset value) => new(value.UtcDateTime);

    private static CalendarProperty DurationProperty(string name, string value)
    {
        var property = new CalendarProperty(name, value);
        property.Parameters.Add("VALUE", "DURATION");
        return property;
    }
}
