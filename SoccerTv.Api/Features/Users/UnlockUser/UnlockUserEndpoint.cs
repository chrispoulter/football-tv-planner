using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Data;

namespace SoccerTv.Api.Features.Users.UnlockUser;

public class UnlockUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/users/{id}/unlock", HandleAsync)
            .RequireRole(Roles.SystemAdministrator, Roles.UserAdministrator)
            .Produces<UnlockUserResponse>()
            .WithTags(Tags.Users)
            .WithSummary("Unlock User")
            .WithDescription("Unlock a user account by ID.");
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
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

        user.IsLockedOut = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new UnlockUserResponse(user.Id));
    }
}
