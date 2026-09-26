using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.ExternalLogin;

public static class ExternalLoginInfoExtensions
{
    /// <summary>
    /// The user's full name from the provider, if it shared one.
    /// </summary>
    public static string? GetName(this ExternalLoginInfo info)
    {
        var name = info.Principal.FindFirstValue(ClaimTypes.Name)?.Trim();

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        return name.Length > UserConfiguration.NameMaxLength
            ? name[..UserConfiguration.NameMaxLength]
            : name;
    }
}
