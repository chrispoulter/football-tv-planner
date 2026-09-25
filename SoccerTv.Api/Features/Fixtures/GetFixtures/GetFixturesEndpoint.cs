using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Fixtures.GetFixtures;

public class GetFixturesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/fixtures", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<GetFixturesRequest>()
            .Produces<GetFixturesResponse>()
            .WithTags(Tags.Fixtures)
            .WithSummary("Get Fixtures")
            .WithDescription(
                "List the televised fixtures kicking off in a UTC time range (typically the viewer's local day), optionally filtered by competition, broadcaster or the current user's schedule."
            );
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetFixturesRequest request,
        CurrentUser? currentUser,
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        if (request.Bookmarked == true && currentUser is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Log in to view your schedule."
            );
        }

        var start = request.From!.Value.ToUniversalTime();
        var end = request.To!.Value.ToUniversalTime();

        var query = dbContext
            .Fixtures.AsNoTracking()
            .Where(f => f.KickoffUtc >= start && f.KickoffUtc < end);

        if (!string.IsNullOrEmpty(request.Competition))
        {
            query = query.Where(f => f.Competition == request.Competition);
        }

        if (!string.IsNullOrEmpty(request.Provider))
        {
            query = query.Where(f => f.Channels.Any(c => c.Provider == request.Provider));
        }

        if (request.Bookmarked == true && currentUser is not null)
        {
            query = query.Where(f =>
                dbContext.UserFixtures.Any(uf =>
                    uf.UserId == currentUser.Id && uf.FixtureId == f.Id
                )
            );
        }

        var fixtures = await query
            .OrderBy(f => f.KickoffUtc)
            .ThenBy(f => f.CompetitionSortOrder)
            .Select(FixtureProjections.ToSummary(dbContext, currentUser?.Id))
            .ToListAsync(cancellationToken);

        return Results.Ok(new GetFixturesResponse(fixtures));
    }
}
