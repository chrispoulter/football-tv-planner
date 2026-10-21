using FootballTvPlanner.Data;
using Microsoft.AspNetCore.DataProtection;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class DataProtectionExtensions
{
    public static IHostApplicationBuilder AddDataProtection(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddDataProtection()
            .SetApplicationName("FootballTvPlanner")
            .PersistKeysToDbContext<FootballTvPlannerDbContext>();

        return builder;
    }
}
