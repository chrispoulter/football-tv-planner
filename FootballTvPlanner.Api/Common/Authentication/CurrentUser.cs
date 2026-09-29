using System.Security.Claims;

namespace FootballTvPlanner.Api.Common.Authentication;

public record CurrentUser(Guid Id)
{
    public static ValueTask<CurrentUser?> BindAsync(HttpContext httpContext)
    {
        if (!Guid.TryParse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
        {
            return ValueTask.FromResult<CurrentUser?>(null);
        }

        return ValueTask.FromResult<CurrentUser?>(new CurrentUser(id));
    }
}
