namespace FootballTvPlanner.Server.Features.Profile.ChangeEmail;

public record ChangeEmailRequested(Guid UserId, string NewEmail);
