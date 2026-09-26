using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.GetLinkedAccounts;

public class GetLinkedAccountsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/account/linked-accounts", HandleAsync)
            .RequireAuthorization()
            .Produces<LinkedAccountsResponse>()
            .WithTags(Tags.Account)
            .WithSummary("Get Linked Accounts")
            .WithDescription(
                "List the external login providers, such as Google, and whether the current user has linked each one."
            );
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        var user = await userManager.FindByIdAsync(currentUser);

        if (user is null)
        {
            return AccountProblems.UserNotFound();
        }

        var logins = await userManager.GetLoginsAsync(user);
        var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();

        var accounts = schemes.Select(s => new LinkedAccount(
            s.Name,
            s.DisplayName ?? s.Name,
            logins.Any(l => l.LoginProvider == s.Name)
        ));

        return Results.Ok(new LinkedAccountsResponse(accounts));
    }
}
