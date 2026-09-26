using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Database;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Data;

public class SoccerTvDbSeeder(UserManager<User> userManager, IOptions<SeedSettings> seedSettings)
    : IDbSeeder<SoccerTvDbContext>
{
    private readonly SeedSettings _seedSettings = seedSettings.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var seedUser in _seedSettings.Users)
        {
            var user = await userManager.FindByEmailAsync(seedUser.EmailAddress);

            if (user is null)
            {
                user = new User
                {
                    UserName = seedUser.EmailAddress,
                    Email = seedUser.EmailAddress,
                    EmailConfirmed = true,
                };

                EnsureSucceeded(await userManager.CreateAsync(user, seedUser.Password));

                continue;
            }

            // Keep the seeded password in sync with configuration
            if (await userManager.HasPasswordAsync(user))
            {
                EnsureSucceeded(await userManager.RemovePasswordAsync(user));
            }

            EnsureSucceeded(await userManager.AddPasswordAsync(user, seedUser.Password));
            EnsureSucceeded(await userManager.SetLockoutEndDateAsync(user, null));
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException($"Failed to seed user: {errors}");
        }
    }
}
