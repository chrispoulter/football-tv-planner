using FootballTvPlanner.Api.Common.Email;
using FootballTvPlanner.Api.Common.Infrastructure;
using FootballTvPlanner.Api.Common.Validation;
using FootballTvPlanner.Api.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Api.Features.Account.Register;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/register", HandleAsync)
            .AllowAnonymous()
            .AddValidationFilter<RegisterRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Register")
            .WithDescription(
                "Create an account, send an email to confirm the address and log the new user in."
            );
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        CancellationToken cancellationToken
    )
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Where(e =>
                e.Code != nameof(IdentityErrorDescriber.DuplicateUserName)
            );

            return errors.ToValidationProblem();
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/account/confirm-email",
            new Dictionary<string, string?> { ["userId"] = user.Id.ToString(), ["code"] = code }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: user.Email,
            subject: "Verify your email address | Football TV Planner",
            template: "FootballTvPlanner.Api.Features.Account.Emails.ConfirmEmail.html",
            model: new { user.Name, Link = link },
            cancellationToken
        );

        await signInManager.SignInAsync(user, isPersistent: false);

        return Results.Ok();
    }
}
