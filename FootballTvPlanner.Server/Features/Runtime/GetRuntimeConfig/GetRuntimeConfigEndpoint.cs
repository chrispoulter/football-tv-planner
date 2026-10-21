using System.Text.Json;
using FootballTvPlanner.Server.Common.Infrastructure;

namespace FootballTvPlanner.Server.Features.Runtime.GetRuntimeConfig;

public class GetRuntimeConfigEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/env.js", HandleAsync).AllowAnonymous().ExcludeFromDescription();
    }

    private static Task<IResult> HandleAsync(
        IConfiguration configuration,
        CancellationToken cancellationToken = default
    ) {
        var env = configuration
            .AsEnumerable()
            .Where(kvp =>
                kvp.Key.StartsWith("VITE_", StringComparison.OrdinalIgnoreCase)
                && kvp.Value is not null
            )
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var json = JsonSerializer.Serialize(env);

        var script = $"window.__ENV__ = {json};";

        return Task.FromResult(Results.Text(script, "application/javascript"));
    }
}
