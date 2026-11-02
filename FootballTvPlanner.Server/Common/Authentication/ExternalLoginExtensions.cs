using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Server.Common.Authentication;

public static class ExternalLoginExtensions
{
    public const string EmailVerifiedClaimType = "email_verified";

    public static string? GetName(this ExternalLoginInfo info)
    {
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        return name;
    }

    public static bool IsEmailVerified(this ExternalLoginInfo info)
    {
        if(! bool.TryParse(info.Principal.FindFirstValue(EmailVerifiedClaimType), out var verified))
        {
            return false;
        }

        return verified;
    }
}