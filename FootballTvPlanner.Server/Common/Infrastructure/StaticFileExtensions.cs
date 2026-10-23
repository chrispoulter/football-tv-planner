using Microsoft.Net.Http.Headers;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class StaticFileExtensions
{
    private const string ImmutableCacheControl = "public, max-age=31536000, immutable";

    private const string RevalidateCacheControl = "no-cache";

    private static readonly StaticFileOptions ClientFileOptions = new()
    {
        OnPrepareResponse = context =>
        {
            context.Context.Response.Headers[HeaderNames.CacheControl] =
                context.Context.Request.Path.StartsWithSegments("/assets")
                    ? ImmutableCacheControl
                    : RevalidateCacheControl;
        },
    };

    public static WebApplication UseClientStaticFiles(this WebApplication app)
    {
        app.UseStaticFiles(ClientFileOptions);

        return app;
    }

    public static WebApplication MapClientFallback(this WebApplication app)
    {
        app.MapFallbackToFile("index.html", ClientFileOptions);

        return app;
    }
}
