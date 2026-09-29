using Microsoft.AspNetCore.Identity;

namespace FootballTvPlanner.Api.Common.Validation;

public static class IdentityResultExtensions
{
    public static IResult ToValidationProblem(this IEnumerable<IdentityError> errors)
    {
        return Results.ValidationProblem(
            errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())
        );
    }
}
