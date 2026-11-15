using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Authentication;
using FootballTvPlanner.Server.Common.Infrastructure;
using FootballTvPlanner.Server.Features.Account.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace FootballTvPlanner.Server.Features.Profile.SendConfirmationEmail;

public class SendConfirmationEmailEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/send-confirmation-email", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .WithSummary("Send Confirmation Email")
            .WithDescription("Send another link to confirm the current user's email address.");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] object empty,
        CurrentUser currentUser,
        UserManager<User> userManager,
        IBus bus
    )
    {
        var user = await userManager.GetUserAsync(currentUser);

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        if (user.EmailConfirmed)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Your email address is already verified."
            );
        }

        await bus.SendLocal(new ConfirmationEmailRequested(user.Id));

        return Results.Ok();
    }
}
