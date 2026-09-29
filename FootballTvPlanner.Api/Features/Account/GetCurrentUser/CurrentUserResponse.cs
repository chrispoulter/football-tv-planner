namespace FootballTvPlanner.Api.Features.Account.GetCurrentUser;

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string? Name,
    bool IsEmailConfirmed,
    bool HasPassword,
    bool IsTwoFactorEnabled
);
