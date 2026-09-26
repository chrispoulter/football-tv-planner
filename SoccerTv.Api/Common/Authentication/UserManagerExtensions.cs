using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Common.Authentication;

public static class UserManagerExtensions
{
    public static Task<User?> FindByIdAsync(
        this UserManager<User> userManager,
        CurrentUser currentUser
    ) => userManager.FindByIdAsync(currentUser.Id.ToString());
}
