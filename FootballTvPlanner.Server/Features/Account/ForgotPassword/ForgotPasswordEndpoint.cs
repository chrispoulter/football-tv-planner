using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Common.Validation;
using Rebus.Bus;

namespace FootballTvPlanner.Server.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/forgot-password", HandleAsync)
            .AllowAnonymous()
            .RequireRateLimiting(RateLimitPolicies.Auth)
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(Tags.Account)
            .WithSummary("Forgot Password")
            .WithDescription("Send a password reset link, if the email belongs to an account.");
    }

    private static async Task<IResult> HandleAsync(ForgotPasswordRequest request, IBus bus)
    {
        await bus.SendLocal(new SendPasswordResetEmail(request.Email));

        return Results.Ok();
    }
}
