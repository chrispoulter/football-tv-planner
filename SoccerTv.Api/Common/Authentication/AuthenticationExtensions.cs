using Microsoft.AspNetCore.Identity;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Users;
using SoccerTv.Api.Features.Account.ExternalLogin;

namespace SoccerTv.Api.Common.Authentication;

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

                // Favour length over composition rules, matching the web app's validation
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<SoccerTvDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // Report 401 and 403 to the web app rather than redirecting to a login page
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

        // Google sign-in is optional so the app still runs without OAuth credentials
        if (!string.IsNullOrEmpty(googleSettings?.ClientId))
        {
            builder
                .Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = googleSettings.ClientId;
                    options.ClientSecret = googleSettings.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;

                    // e.g. the user cancelled on the consent screen
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

        builder.Services.AddScoped<AccountEmailSender>();
        builder.Services.AddAuthorization();

        return builder;
    }
}
