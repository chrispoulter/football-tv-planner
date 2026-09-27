using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;

namespace SoccerTv.Api.Features.Account;

public static class ExternalLoginRedirects
{
    private const string XsrfKey = "XsrfId";

    /// <summary>
    /// Only allow redirects back to a path on the web app, never to another site.
    /// </summary>
    public static string LocalOrRoot(string? returnUrl) =>
        returnUrl is ['/', not '/' and not '\\', ..] or "/" ? returnUrl : "/";

    public static string LoginError(string error) => WithError("/account/login", error);

    public static string WithError(string returnUrl, string error) =>
        QueryHelpers.AddQueryString(returnUrl, "error", error);

    public static string RemoteFailure(AuthenticationProperties? properties)
    {
        if (properties?.Items.ContainsKey(XsrfKey) == true && properties.RedirectUri is { } uri)
        {
            var query = QueryHelpers.ParseQuery(new Uri(new Uri("http://localhost"), uri).Query);

            return WithError(LocalOrRoot(query["returnUrl"]), "link");
        }

        return LoginError("external");
    }
}
