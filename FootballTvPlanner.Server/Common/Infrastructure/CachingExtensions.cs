using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class CachingExtensions
{
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache(options => options.SizeLimit = 20 * 1024 * 1024);

        builder.Services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1),
                LocalCacheExpiration = TimeSpan.FromHours(1),
            };
        });

        return builder;
    }
}
