using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Competitions;
using FootballTvPlanner.Api.Data.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Fixtures.GetCompetitions;

public class GetCompetitionsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/competitions", HandleAsync)
            .AllowAnonymous()
            .Produces<List<CompetitionResponse>>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Competitions")
            .WithDescription("List the competitions that have televised fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var fixtures = dbContext.Fixtures.Visible();

        var competitions = await dbContext
            .Competitions.AsNoTracking()
            .Where(c => fixtures.Any(f => f.CompetitionId == c.Id))
            .InDisplayOrder()
            .Select(c => new CompetitionResponse(c.Id, c.DisplayName ?? c.Name))
            .ToListAsync(cancellationToken);

        return Results.Ok(competitions);
    }
}
