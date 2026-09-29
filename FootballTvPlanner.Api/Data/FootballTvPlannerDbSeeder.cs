using FootballTvPlanner.Api.Common.Database;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Api.Data;

public class FootballTvPlannerDbSeeder(
    UserManager<User> userManager,
    IOptions<SeedSettings> seedSettings
) : IDbSeeder<FootballTvPlannerDbContext>
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
                    Name = seedUser.Name,
                };

                EnsureSucceeded(await userManager.CreateAsync(user, seedUser.Password));

                continue;
            }

            user.UserName = seedUser.EmailAddress;
            user.Email = seedUser.EmailAddress;
            user.EmailConfirmed = true;
            user.Name = seedUser.Name;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            EnsureSucceeded(await userManager.UpdateAsync(user));

            if (await userManager.HasPasswordAsync(user))
            {
                EnsureSucceeded(await userManager.RemovePasswordAsync(user));
            }

            EnsureSucceeded(await userManager.AddPasswordAsync(user, seedUser.Password));
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
