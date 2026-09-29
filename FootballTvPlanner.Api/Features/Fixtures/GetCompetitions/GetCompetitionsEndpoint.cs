using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Fixtures.GetCompetitions;

public class GetCompetitionsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/competitions", HandleAsync)
            .AllowAnonymous()
            .Produces<List<string>>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Competitions")
            .WithDescription("List the competitions that have televised fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var competitions = await dbContext
            .Fixtures.AsNoTracking()
            .Select(f => f.Competition)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

        return Results.Ok(competitions);
    }
}
