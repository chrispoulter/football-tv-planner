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
            .Services.AddIdentityApiEndpoints<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                // Favour length over composition rules, matching the web app's validation
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<SoccerTvDbContext>();

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
                        context.Response.Redirect(ExternalLoginRedirects.LoginError("external"));
                        context.HandleResponse();

                        return Task.CompletedTask;
                    };
                });
        }

        builder.Services.AddSingleton<IEmailSender<User>, IdentityEmailSender>();
        builder.Services.AddAuthorization();

        return builder;
    }
}
