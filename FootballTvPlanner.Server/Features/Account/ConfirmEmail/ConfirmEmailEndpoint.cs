using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Common.Validation;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Server.Features.Account.ConfirmEmail;

public class ConfirmEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/confirm-email", HandleAsync)
            .RequireRateLimiting(RateLimitPolicies.Auth)
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

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This link is invalid or has expired."
            );
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await userManager.ConfirmEmailAsync(user, request.Code);
        }
        else
        {
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, request.Code);

            if (result.Succeeded)
            {
                result = await userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        if (!result.Succeeded)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This link is invalid or has expired."
            );
        }

        if (currentUser?.Id == user.Id)
        {
            await signInManager.RefreshSignInAsync(user);
        }

        return Results.Ok();
    }
}
