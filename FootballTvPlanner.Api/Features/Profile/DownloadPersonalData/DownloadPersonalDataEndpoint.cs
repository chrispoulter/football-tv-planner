using System.Text.Encodings.Web;
using System.Text.Json;
using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Api.Features.Profile.DownloadPersonalData;

public class DownloadPersonalDataEndpoint : IEndpoint
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/personal-data", HandleAsync)
            .RequireAuthorization()
            .Produces<PersonalDataResponse>(contentType: "application/json")
            .WithTags(Tags.Profile)
            .WithSummary("Download Personal Data")
            .WithDescription(
                "Download a copy of the personal data held for the current user as a JSON file. Passwords and two-factor secrets are not included."
            );
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        UserManager<User> userManager,
        FootballTvPlannerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.GetUserAsync(currentUser);

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var logins = await userManager.GetLoginsAsync(user);

        var bookmarks = await dbContext
            .UserFixtures.AsNoTracking()
            .IgnoreQueryFilters()
            .Where(uf => uf.UserId == user.Id)
            .OrderBy(uf => uf.Fixture.KickoffUtc)
            .Select(uf => new PersonalDataBookmark(
                uf.Fixture.HomeTeam.Name,
                uf.Fixture.AwayTeam.Name,
                uf.Fixture.Competition.Name,
                uf.Fixture.KickoffUtc,
                uf.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var response = new PersonalDataResponse(
            user.Id,
            user.Name,
            user.Email,
            user.EmailConfirmed,
            user.TwoFactorEnabled,
            logins.Select(l => l.LoginProvider),
            user.CalendarFeedToken,
            bookmarks
        );

        return Results.File(
            JsonSerializer.SerializeToUtf8Bytes(response, JsonOptions),
            "application/json",
            "football-tv-planner-data.json"
        );
    }
}
