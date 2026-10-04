using FootballTvPlanner.Api.Common.Authentication;
using FootballTvPlanner.Api.Data;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Common.Authentication;

public static class AuthenticationExtensions
{
    public static IHostApplicationBuilder AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder
            .Services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<FootballTvPlannerDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        var googleSettings = builder
            .Configuration.GetSection(GoogleSettings.SectionName)
            .Get<GoogleSettings>();

        if (!string.IsNullOrEmpty(googleSettings?.ClientId))
        {
            builder
                .Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = googleSettings.ClientId;
                    options.ClientSecret = googleSettings.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.CallbackPath = "/api/signin-google";

                    options.Events.OnRemoteFailure = context =>
                    {
                        context.Response.Redirect(
                            ExternalLoginRedirects.RemoteFailure(context.Properties)
                        );
                        context.HandleResponse();

                        return Task.CompletedTask;
                    };
                });
        }

        builder.Services.AddAuthorization();

        return builder;
    }
}
