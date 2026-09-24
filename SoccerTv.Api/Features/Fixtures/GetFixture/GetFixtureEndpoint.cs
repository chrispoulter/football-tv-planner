using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Fixtures.GetFixture;

public class GetFixtureEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/fixtures/{id:guid}", HandleAsync)
            .AllowAnonymous()
            .Produces<FixtureSummary>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Fixture")
            .WithDescription("Retrieve a single fixture by ID.");
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        CurrentUser? currentUser,
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var fixture = await dbContext
            .Fixtures.AsNoTracking()
            .Where(f => f.Id == id)
            .Select(FixtureProjections.ToSummary(dbContext, currentUser?.Id))
            .FirstOrDefaultAsync(cancellationToken);

        if (fixture is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Fixture not found."
            );
        }

        return Results.Ok(fixture);
    }
}
