using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.Register;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/register", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<RegisterRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Register")
            .WithDescription(
                "Create an account, send an email to confirm the address and log the new user in."
            );
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        AccountEmailSender emailSender
    )
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name.Trim(),
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // The user name is the email, so only report the duplicate email
            return IdentityResult
                .Failed([
                    .. result.Errors.Where(e =>
                        e.Code != nameof(IdentityErrorDescriber.DuplicateUserName)
                    ),
                ])
                .ToValidationProblem();
        }

        await emailSender.SendConfirmationLinkAsync(user);

        // The email doesn't need confirming before logging in
        await signInManager.SignInAsync(user, isPersistent: false);

        return Results.Ok();
    }
}
