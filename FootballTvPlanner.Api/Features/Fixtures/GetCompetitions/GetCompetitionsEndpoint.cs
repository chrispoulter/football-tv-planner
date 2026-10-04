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
            .Produces<GetCompetitionsResponse>()
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
            .Competitions.AsNoTracking()
            .Where(c => dbContext.Fixtures.Any(f => f.CompetitionId == c.Id))
            .OrderBy(c => c.Name)
            .Select(c => new GetCompetitionsItem(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return Results.Ok(new GetCompetitionsResponse(competitions));
    }
}
