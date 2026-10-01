namespace FootballTvPlanner.FixtureSync.Providers;

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
