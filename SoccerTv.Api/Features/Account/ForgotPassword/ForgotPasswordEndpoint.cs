using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Email;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/forgot-password", HandleAsync)
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Forgot Password")
            .WithDescription("Initiate the password reset process for a user.");
    }

    private static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        SoccerTvDbContext dbContext,
        IHashService hashService,
        IEmailService emailService,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmailAddress = request.EmailAddress.ToLowerInvariant();

        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.NormalizedEmailAddress == normalizedEmailAddress,
            cancellationToken
        );

        if (user is not null && !user.IsLockedOut)
        {
            var passwordResetToken = Convert
                .ToHexString(RandomNumberGenerator.GetBytes(16))
                .ToUpperInvariant();

            user.PasswordResetToken = hashService.GenerateHash(passwordResetToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await emailService.SendTemplateEmailAsync(
                toAddress: user.EmailAddress,
                subject: "Reset Password // SoccerTv",
                template: "SoccerTv.Api.Features.Account.ForgotPassword.ResetPasswordEmail.html",
                model: new { PasswordResetToken = passwordResetToken },
                cancellationToken
            );
        }

        return Results.Ok();
    }
}
