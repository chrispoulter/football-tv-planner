using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Common.Authentication;

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
