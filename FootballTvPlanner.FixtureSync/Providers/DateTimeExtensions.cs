namespace FootballTvPlanner.FixtureSync.Providers;

public static class DateTimeExtensions
{
    public static TimeZoneInfo TimeZone { get; } =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    public static DateTimeOffset ToUtc(DateOnly date, TimeOnly time)
    {
        var local = date.ToDateTime(time);

        // Times in the spring-forward gap don't exist; treat them as the hour after.
        if (TimeZone.IsInvalidTime(local))
        {
            local = local.AddHours(1);
        }

        var utc = TimeZoneInfo.ConvertTimeToUtc(local, TimeZone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }
}
