namespace FootballTvPlanner.Server.Features.Profile.ChangeEmail;

public record SendChangeEmailConfirmation(Guid UserId, string NewEmail);
