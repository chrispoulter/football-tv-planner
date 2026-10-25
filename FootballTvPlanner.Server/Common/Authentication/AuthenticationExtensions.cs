using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Server.Common.Authentication;

public static class AuthenticationExtensions
{
    public static IHostApplicationBuilder AddAuthentication<TUser, TContext>(
        this IHostApplicationBuilder builder
    )
        where TUser : class
        where TContext : DbContext
    {
        builder
            .Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder
            .Services.AddIdentityCore<TUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<TContext>()
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
