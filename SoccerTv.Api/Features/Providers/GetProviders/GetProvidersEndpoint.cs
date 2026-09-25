using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Providers.GetProviders;

public class GetProvidersEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/providers", HandleAsync)
            .AllowAnonymous()
            .Produces<List<string>>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Providers")
            .WithDescription("List the broadcasters showing fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        // Channels are stored as JSON, so flatten them here rather than in SQL.
        var channels = await dbContext
            .Fixtures.AsNoTracking()
            .Select(f => f.Channels)
            .ToListAsync(cancellationToken);

        var providers = channels
            .SelectMany(c => c)
            .Select(c => c.Provider)
            .Distinct()
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Results.Ok(providers);
    }
}
