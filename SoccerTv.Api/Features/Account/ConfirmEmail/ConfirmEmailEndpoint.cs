using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ConfirmEmail;

public class ConfirmEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/confirm-email", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<ConfirmEmailRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Confirm Email")
            .WithDescription(
                "Confirm an email address, or complete an email change, using the link from the confirmation email."
            );
    }

    private static async Task<IResult> HandleAsync(
        ConfirmEmailRequest request,
        CurrentUser? currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        var code = AccountCodes.Decode(request.Code);

        if (user is null || code is null)
        {
            return InvalidLink();
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, code);

            // The email is also the user name
            if (result.Succeeded)
            {
                result = await userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        if (!result.Succeeded)
        {
            return InvalidLink();
        }

        // Changing the email updates the security stamp, which would otherwise log the user out
        if (currentUser?.Id == user.Id)
        {
            await signInManager.RefreshSignInAsync(user);
        }

        return Results.Ok();
    }

    private static IResult InvalidLink() =>
        AccountProblems.BadRequest("This link is invalid or has expired.");
}
