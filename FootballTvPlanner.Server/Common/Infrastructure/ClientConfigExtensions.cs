using System.Text.Json;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class ClientConfigExtensions
{
    public static WebApplication MapClientConfig(this WebApplication app)
    {
        app.MapGet(
                "/env.js",
                (IConfiguration configuration, HttpResponse response) =>
                {
                    response.Headers.CacheControl = "no-store";

                    var env = configuration
                        .AsEnumerable()
                        .Where(kvp =>
                            kvp.Key.StartsWith("VITE_", StringComparison.OrdinalIgnoreCase)
                            && kvp.Value is not null
                        )
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    var json = JsonSerializer.Serialize(env);

                    var script = $"window.__ENV__ = {json};";

                    return Results.Text(script, "application/javascript");
                }
            )
            .AllowAnonymous()
            .ExcludeFromDescription();

        return app;
    }
}
