namespace FootballTvPlanner.Server.Features.Profile.TwoFactor.GetTwoFactor;

public record TwoFactorResponse(bool IsEnabled, int RecoveryCodesLeft);
