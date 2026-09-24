using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Competitions.GetCompetitions;

public class GetCompetitionsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/competitions", HandleAsync)
            .AllowAnonymous()
            .Produces<List<GetCompetitionsResponse>>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Competitions")
            .WithDescription("List the competitions that have televised fixtures.");
    }

    private static async Task<IResult> HandleAsync(
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var competitions = await dbContext
            .Competitions.AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new GetCompetitionsResponse(c.Id, c.Name, c.ShortName, c.Country))
            .ToListAsync(cancellationToken);

        return Results.Ok(competitions);
    }
}
