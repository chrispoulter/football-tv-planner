using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.Login;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/login", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<LoginRequest>()
            .Produces<LoginResponse>()
            .WithTags(Tags.Account)
            .WithSummary("Login")
            .WithDescription(
                "Log in with an email address and password. If two-factor authentication is enabled, the login is completed with a code."
            );
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The credentials provided were invalid."
            );
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: true
        );

        if (result.RequiresTwoFactor)
        {
            return Results.Ok(new LoginResponse(RequiresTwoFactor: true));
        }

        if (result.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This account has been locked out, please try again later."
            );
        }

        if (!result.Succeeded)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The credentials provided were invalid."
            );
        }

        return Results.Ok(new LoginResponse(RequiresTwoFactor: false));
    }
}
