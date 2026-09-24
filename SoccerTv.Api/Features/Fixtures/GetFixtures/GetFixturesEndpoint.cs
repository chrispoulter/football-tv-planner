using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Time;
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
                "List the televised fixtures for a UK day, optionally filtered by competition or broadcaster."
            );
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetFixturesRequest request,
        CurrentUser? currentUser,
        SoccerTvDbContext dbContext,
        TimeProvider timeProvider,
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

        var date = request.Date ?? UkTime.Today(timeProvider);
        var (start, end) = UkTime.DayBoundsUtc(date);

        var query = dbContext
            .Fixtures.AsNoTracking()
            .Where(f => f.KickoffUtc >= start && f.KickoffUtc < end);

        if (request.CompetitionId is not null)
        {
            query = query.Where(f => f.CompetitionId == request.CompetitionId);
        }

        if (!string.IsNullOrEmpty(request.Provider))
        {
            query = query.Where(f => f.Broadcasts.Any(b => b.Channel.Provider == request.Provider));
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
            .ThenBy(f => f.Competition.SortOrder)
            .Select(FixtureProjections.ToSummary(dbContext, currentUser?.Id))
            .ToListAsync(cancellationToken);

        return Results.Ok(new GetFixturesResponse(date, fixtures));
    }
}
