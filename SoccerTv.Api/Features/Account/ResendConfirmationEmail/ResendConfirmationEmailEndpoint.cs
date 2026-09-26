using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ResendConfirmationEmail;

public class ResendConfirmationEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/confirm-email/resend", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Account)
            .WithSummary("Resend Confirmation Email")
            .WithDescription("Send another email to confirm the current user's email address.");
    }

    // Requiring a JSON body means a cross-site form post can't trigger this
    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager,
        AccountEmailSender emailSender
    )
    {
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        if (user.EmailConfirmed)
        {
            return AccountProblems.BadRequest("Your email address has already been confirmed.");
        }

        await emailSender.SendConfirmationLinkAsync(user);

        return Results.Ok();
    }
}
