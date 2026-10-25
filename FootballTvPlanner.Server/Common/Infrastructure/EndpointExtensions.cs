using System.Reflection;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class EndpointExtensions
{
    public static RouteGroupBuilder MapApiEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpoints = assembly.DefinedTypes.Where(type =>
            type is { IsAbstract: false, IsInterface: false }
            && typeof(IEndpoint).IsAssignableFrom(type)
        );

        var api = app.MapGroup("/api");

        foreach (var endpoint in endpoints)
        {
            if (Activator.CreateInstance(endpoint) is IEndpoint instance)
            {
                instance.MapEndpoints(api);
            }
        }

        api.MapFallback(() => Results.NotFound()).ExcludeFromDescription();

        return api;
    }
}
