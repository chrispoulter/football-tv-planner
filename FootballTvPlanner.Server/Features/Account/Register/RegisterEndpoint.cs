using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Common.Validation;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Server.Features.Account.Register;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/register", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<RegisterRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Register")
            .WithDescription("Create an account and log the new user in.");
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Where(e =>
                e.Code != nameof(IdentityErrorDescriber.DuplicateUserName)
            );

            return errors.ToValidationProblem();
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        return Results.Ok();
    }
}
