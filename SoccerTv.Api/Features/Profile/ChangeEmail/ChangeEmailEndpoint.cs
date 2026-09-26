using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
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
        AccountEmailSender emailSender
    )
    {
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        var existingUser = await userManager.FindByEmailAsync(request.NewEmail);

        if (existingUser?.Id == user.Id)
        {
            return AccountProblems.Validation(
                nameof(request.NewEmail),
                "This is already your email address."
            );
        }

        if (existingUser is not null)
        {
            return IdentityResult
                .Failed(userManager.ErrorDescriber.DuplicateEmail(request.NewEmail))
                .ToValidationProblem();
        }

        await emailSender.SendChangeEmailLinkAsync(user, request.NewEmail);

        return Results.Ok();
    }
}
