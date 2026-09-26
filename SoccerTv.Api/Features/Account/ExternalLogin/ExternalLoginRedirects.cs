namespace SoccerTv.Api.Features.Account.ExternalLogin;

public static class ExternalLoginRedirects
{
    /// <summary>
    /// Only allow redirects back to a path on the web app, never to another site.
    /// </summary>
    public static string LocalOrRoot(string? returnUrl) =>
        returnUrl is ['/', not '/' and not '\\', ..] or "/" ? returnUrl : "/";

    public static string LoginError(string error) => $"/account/login?error={error}";
}
