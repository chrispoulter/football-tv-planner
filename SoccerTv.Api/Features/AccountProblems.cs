namespace SoccerTv.Api.Features;

public static class AccountProblems
{
    public static IResult UserNotFound() =>
        Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "User not found.");

    public static IResult BadRequest(string title) =>
        Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: title);

    public static IResult Unauthorized(string title) =>
        Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: title);

    public static IResult Validation(string key, string message) =>
        Results.ValidationProblem(new Dictionary<string, string[]> { [key] = [message] });
}
