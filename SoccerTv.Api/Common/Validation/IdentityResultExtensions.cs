using Microsoft.AspNetCore.Identity;

namespace SoccerTv.Api.Common.Validation;

public static class IdentityResultExtensions
{
    public static IResult ToValidationProblem(this IdentityResult result) =>
        Results.ValidationProblem(
            result
                .Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())
        );
}
