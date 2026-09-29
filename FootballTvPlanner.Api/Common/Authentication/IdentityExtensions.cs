using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Common.Authentication;

public static class IdentityExtensions
{
    public static Task<TUser?> GetUserAsync<TUser>(
        this UserManager<TUser> userManager,
        CurrentUser currentUser
    )
        where TUser : User
    {
        return userManager.FindByIdAsync(currentUser.Id.ToString());
    }
}
