using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Server.Common.Authentication;

public static class IdentityExtensions
{
    public static Task<TUser?> GetUserAsync<TUser>(
        this UserManager<TUser> userManager,
        CurrentUser currentUser
    )
        where TUser : class
    {
        return userManager.FindByIdAsync(currentUser.Id.ToString());
    }
}
