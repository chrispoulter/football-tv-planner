using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Profile.ChangeEmail;

public class ChangeEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/change-email", HandleAsync)
            .RequireAuthorization()
            .AddValidationFilter<ChangeEmailRequest>()
            .WithTags(Tags.Profile)
            .WithSummary("Change Email")
            .WithDescription(
                "Send a link to the new email address. The email only changes once the link is followed."
            );
    }

    private static async Task<IResult> HandleAsync(
        ChangeEmailRequest request,
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

        var existingUser = await userManager.FindByEmailAsync(request.NewEmail);

        if (existingUser?.Id == user.Id)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This is already your email address."
            );
        }

        if (existingUser is not null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This email address is already in use."
            );
        }

        var code = await userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/account/confirm-email",
            new Dictionary<string, string?>
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = code,
                ["changedEmail"] = request.NewEmail,
            }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: request.NewEmail,
            subject: "Verify your email address | Soccer TV",
            template: "SoccerTv.Api.Features.Emails.ConfirmEmail.html",
            model: new { user.Name, Link = link },
            cancellationToken
        );

        return Results.Ok();
    }
}
