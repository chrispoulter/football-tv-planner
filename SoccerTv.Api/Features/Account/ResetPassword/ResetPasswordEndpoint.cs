using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/reset-password", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<ResetPasswordRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Reset Password")
            .WithDescription("Set a new password using the link from the password reset email.");
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        UserManager<User> userManager
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This link is invalid or has expired."
            );
        }

        var result = await userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);

        if (!result.Succeeded)
        {
            var errorDictionary = result
                .Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return Results.ValidationProblem(errorDictionary);
        }

        return Results.Ok();
    }
}
