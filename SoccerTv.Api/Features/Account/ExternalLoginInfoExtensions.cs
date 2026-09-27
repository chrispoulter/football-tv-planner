using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account;

public static class ExternalLoginInfoExtensions
{
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
