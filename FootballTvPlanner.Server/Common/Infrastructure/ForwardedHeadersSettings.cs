using Microsoft.AspNetCore.HttpOverrides;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public class ForwardedHeadersSettings
{
    public static string SectionName { get; } = "ForwardedHeaders";

    public string ForwardedForHeaderName { get; set; } =
        ForwardedHeadersDefaults.XForwardedForHeaderName;
}
