using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.ResendConfirmationEmail;

public class ResendConfirmationEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/confirm-email/resend", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Resend Confirmation Email")
            .WithDescription("Send another email to confirm the current user's email address.");
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
        var user = await userManager.FindByIdAsync(currentUser.Id.ToString());

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
                title: "Your email address has already been confirmed."
            );
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/account/confirm-email",
            new Dictionary<string, string?> { ["userId"] = user.Id.ToString(), ["code"] = code }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: user.Email!,
            subject: "Verify your email address | Soccer TV",
            template: "SoccerTv.Api.Features.Emails.ConfirmEmail.html",
            model: new { user.Name, Link = link },
            cancellationToken
        );

        return Results.Ok();
    }
}
