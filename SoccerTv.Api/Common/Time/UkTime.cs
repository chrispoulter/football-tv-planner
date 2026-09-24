namespace SoccerTv.Api.Common.Time;

/// <summary>
/// UK broadcasters publish kick-off times in UK local time. Providers use this to convert
/// them to UTC on the way in; everything stored and returned by the API is UTC.
/// </summary>
public static class UkTime
{
    public static TimeZoneInfo TimeZone { get; } =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    public static DateTimeOffset ToUtc(DateOnly date, TimeOnly time)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(time), TimeZone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }
}
