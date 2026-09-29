namespace FootballTvPlanner.Api.Common.Authentication;

public class GoogleSettings
{
    public static string SectionName { get; } = "Authentication:Google";

    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;
}
