using Microsoft.EntityFrameworkCore;
using SoccerTv.Api.Common.Authentication;
using SoccerTv.Api.Common.Infrastructure;
using SoccerTv.Api.Common.Validation;
using SoccerTv.Api.Data;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.Register;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/register", HandleAsync)
            .AddValidationFilter<RegisterRequest>()
            .Produces<RegisterResponse>()
            .WithTags(Tags.Account)
            .WithSummary("Register")
            .WithDescription("Register a new user account.");
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
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
        };

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new RegisterResponse(user.Id));
    }
}
