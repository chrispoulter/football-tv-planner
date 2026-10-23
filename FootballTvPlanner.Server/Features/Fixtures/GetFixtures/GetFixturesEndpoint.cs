using FootballTvPlanner.Data;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Common.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FootballTvPlanner.Server.Features.Fixtures.GetFixtures;

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
                "List the televised fixtures kicking off in a UTC time range (typically the viewer's local day), optionally filtered by competition, channel or the current user's schedule."
            );
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetFixturesRequest request,
        CurrentUser? currentUser,
        FootballTvPlannerDbContext dbContext,
        HybridCache cache,
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

        var response = await cache.GetOrCreateAsync(
            $"fixture:{start:o}:{end:o}:{request.CompetitionId}:{request.ChannelId}:{request.Bookmarked}:{currentUser?.Id}",
            async cancel =>
            {
                var query = dbContext
                    .Fixtures.AsNoTracking()
                    .Where(f => f.KickoffUtc >= start && f.KickoffUtc < end);

                if (request.CompetitionId is { } competitionId)
                {
                    query = query.Where(f => f.CompetitionId == competitionId);
                }

                if (request.ChannelId is { } channelId)
                {
                    query = query.Where(f => f.Channels.Any(c => c.Id == channelId));
                }

                if (request.Bookmarked == true && currentUser is not null)
                {
                    query = query.Where(f =>
                        dbContext.UserFixtures.Any(uf =>
                            uf.UserId == currentUser.Id && uf.FixtureId == f.Id
                        )
                    );
                }

                var userId = currentUser?.Id;

                var fixtures = await query
                    .OrderBy(f => f.Competition.Name)
                    .ThenBy(f => f.KickoffUtc)
                    .Select(f => new GetFixturesItem(
                        f.Id,
                        f.KickoffUtc,
                        new GetFixturesCompetition(f.Competition.Id, f.Competition.Name),
                        new GetFixturesTeam(f.HomeTeam.Id, f.HomeTeam.Name),
                        new GetFixturesTeam(f.AwayTeam.Id, f.AwayTeam.Name),
                        f.Channels.OrderBy(c => c.Name)
                            .Select(c => new GetFixturesChannel(c.Id, c.Name))
                            .ToList(),
                        userId != null
                            && dbContext.UserFixtures.Any(uf =>
                                uf.UserId == userId && uf.FixtureId == f.Id
                            )
                    ))
                    .ToListAsync(cancel);

                return new GetFixturesResponse(fixtures);
            },
            tags: currentUser is null ? [] : [$"user:{currentUser.Id}"],
            cancellationToken: cancellationToken
        );

        return Results.Ok(response);
    }
}
