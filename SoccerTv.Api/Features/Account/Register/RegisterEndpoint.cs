using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.Register;

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

            return Results.ValidationProblem(
                errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())
            );
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = QueryHelpers.AddQueryString(
            $"{emailSettings.Value.SiteUrl}/account/confirm-email",
            new Dictionary<string, string?> { ["userId"] = user.Id.ToString(), ["code"] = code }
        );

        await emailService.SendTemplateEmailAsync(
            toAddress: user.Email,
            subject: "Verify your email address | Soccer TV",
            template: "SoccerTv.Api.Features.Emails.ConfirmEmail.html",
            model: new { name = user.Name, link },
            cancellationToken
        );

        await signInManager.SignInAsync(user, isPersistent: false);

        return Results.Ok();
    }
}
