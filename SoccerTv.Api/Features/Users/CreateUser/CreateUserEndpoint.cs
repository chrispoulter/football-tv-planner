using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", HandleAsync)
            .RequireRole(Roles.SystemAdministrator, Roles.UserAdministrator)
            .AddValidationFilter<CreateUserRequest>()
            .Produces<CreateUserResponse>()
            .WithTags(Tags.Users)
            .WithSummary("Create User")
            .WithDescription("Create a new user account.");
    }

    private static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        SoccerTvDbContext dbContext,
        IHashService hashService,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmailAddress = request.EmailAddress.ToLowerInvariant();

        var existing = await dbContext.Users.AnyAsync(
            u => u.NormalizedEmailAddress == normalizedEmailAddress,
            cancellationToken
        );

        if (existing)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "User name is already taken."
            );
        }

        var user = new User
        {
            EmailAddress = request.EmailAddress,
            Password = hashService.GenerateHash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Roles = request.Roles,
        };

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new CreateUserResponse(user.Id));
    }
}
