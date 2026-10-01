namespace FootballTvPlanner.Api.Features.Profile.TwoFactor.GetTwoFactor;

public record TwoFactorResponse(bool IsEnabled, int RecoveryCodesLeft);
