using FootballTvPlanner.Data;
using FootballTvPlanner.Server.Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features.Fixtures.GetCompetitions;

public class GetCompetitionsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/competitions", HandleAsync)
            .AllowAnonymous()
            .Produces<GetCompetitionsResponse>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Competitions")
            .WithDescription("List the competitions that have upcoming televised fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        FootballTvPlannerDbContext dbContext,
        HybridCache cache,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var response = await cache.GetOrCreateAsync(
            "competitions",
            async cancel =>
            {
                var cutoff = timeProvider.GetUtcNow().AddDays(-1);

                var competitions = await dbContext
                    .Competitions.AsNoTracking()
                    .Where(c =>
                        dbContext.Fixtures.Any(f =>
                            f.CompetitionId == c.Id && f.KickoffUtc >= cutoff
                        )
                    )
                    .OrderBy(c => c.Name)
                    .Select(c => new GetCompetitionsItem(c.Id, c.Name))
                    .ToListAsync(cancel);

                return new GetCompetitionsResponse(competitions);
            },
            cancellationToken: cancellationToken
        );

        return Results.Ok(response);
    }
}
