using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Fixtures.GetChannels;

public class GetChannelsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/channels", HandleAsync)
            .AllowAnonymous()
            .Produces<List<string>>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Channels")
            .WithDescription("List the TV channels and streaming services showing fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var channels = await dbContext
            .Fixtures.AsNoTracking()
            .SelectMany(f => f.Channels)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

        return Results.Ok(channels);
    }
}
