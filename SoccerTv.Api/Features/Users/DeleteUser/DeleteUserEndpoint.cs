using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Users.DeleteUser;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/users/{id}", HandleAsync)
            .RequireRole(Roles.SystemAdministrator, Roles.UserAdministrator)
            .Produces<DeleteUserResponse>()
            .WithTags(Tags.Users)
            .WithSummary("Delete User")
            .WithDescription("Delete a user account by ID.");
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        CurrentUser currentUser,
        SoccerTvDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        if (user.Id == currentUser.Id)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Cannot delete currently logged in user."
            );
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new DeleteUserResponse(user.Id));
    }
}
