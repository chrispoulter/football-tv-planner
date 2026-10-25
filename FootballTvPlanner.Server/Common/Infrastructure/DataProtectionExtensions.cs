using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class DataProtectionExtensions
{
    public static IHostApplicationBuilder AddDataProtection<TContext>(
        this IHostApplicationBuilder builder
    )
        where TContext : DbContext, IDataProtectionKeyContext
    {
        builder
            .Services.AddDataProtection()
            .SetApplicationName(builder.Environment.ApplicationName)
            .PersistKeysToDbContext<TContext>();

        return builder;
    }
}
