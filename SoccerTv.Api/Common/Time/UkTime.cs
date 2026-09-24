namespace SoccerTv.Api.Common.Time;

public static class UkTime
{
    public static TimeZoneInfo TimeZone { get; } =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    public static DateOnly Today(TimeProvider timeProvider)
    {
        var local = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), TimeZone);
        return DateOnly.FromDateTime(local.DateTime);
    }

    public static DateTimeOffset ToUtc(DateOnly date, TimeOnly time)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(time), TimeZone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }

    public static (DateTimeOffset Start, DateTimeOffset End) DayBoundsUtc(DateOnly date)
    {
        return (ToUtc(date, TimeOnly.MinValue), ToUtc(date.AddDays(1), TimeOnly.MinValue));
    }
}
