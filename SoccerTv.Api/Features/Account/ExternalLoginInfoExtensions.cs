using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace SoccerTv.Api.Features.Account;

public static class ExternalLoginInfoExtensions
{
    public static string? GetName(this ExternalLoginInfo info)
    {
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        return name[..100];
    }
}
