using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Common.Validation;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Features.Account.ResetPassword;

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

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This link is invalid or has expired."
            );
        }

        var result = await userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);

        if (!result.Succeeded)
        {
            return result.Errors.ToValidationProblem();
        }

        return Results.Ok();
    }
}
