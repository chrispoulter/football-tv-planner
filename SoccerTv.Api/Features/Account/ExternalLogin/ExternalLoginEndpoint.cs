using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ExternalLogin;

public class ExternalLoginEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/account/external-login", HandleAsync)
            .WithTags(Tags.Account)
            .WithSummary("External Login")
            .WithDescription("Redirect to an external login provider such as Google.");
    }

    private static async Task<IResult> HandleAsync(
        string provider,
        string? returnUrl,
        HttpContext httpContext,
        SignInManager<User> signInManager
    )
    {
        var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();

        if (!schemes.Any(s => s.Name == provider))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Unknown login provider."
            );
        }

        var callbackUrl = QueryHelpers.AddQueryString(
            $"{httpContext.Request.PathBase}/account/external-login/callback",
            "returnUrl",
            ExternalLoginRedirects.LocalOrRoot(returnUrl)
        );

        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            callbackUrl
        );

        return Results.Challenge(properties, [provider]);
    }
}
