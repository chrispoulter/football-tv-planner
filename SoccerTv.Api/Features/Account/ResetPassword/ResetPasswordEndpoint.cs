using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Account.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/reset-password", HandleAsync)
            .AddValidationFilter<ResetPasswordRequest>()
            .Produces<ResetPasswordResponse>()
            .WithTags(Tags.Account)
            .WithSummary("Reset Password")
            .WithDescription("Reset a user's password using a valid token.");
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        SoccerTvDbContext dbContext,
        IHashService hashService,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmailAddress = request.EmailAddress.ToLowerInvariant();

        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.NormalizedEmailAddress == normalizedEmailAddress,
            cancellationToken
        );

        if (user is null || user.IsLockedOut || string.IsNullOrEmpty(user.PasswordResetToken))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid token."
            );
        }

        var verified = hashService.VerifyHash(request.Token, user.PasswordResetToken);

        if (!verified)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid token."
            );
        }

        user.Password = hashService.GenerateHash(request.NewPassword);
        user.PasswordResetToken = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new ResetPasswordResponse(user.Id));
    }
}
