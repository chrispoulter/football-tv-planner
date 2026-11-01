using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Email;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Features.Account.ConfirmEmail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Features.Profile.ResendConfirmationEmail;

public class ResendConfirmationEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/resend-confirmation-email", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Resend Confirmation Email")
            .WithDescription("Send another link to confirm the current user's email address.");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
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

        if (user.EmailConfirmed)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Your email address is already verified."
            );
        }

        await ConfirmationEmail.SendAsync(
            user,
            userManager,
            emailService,
            emailSettings.Value,
            cancellationToken
        );

        return Results.Ok();
    }
}
