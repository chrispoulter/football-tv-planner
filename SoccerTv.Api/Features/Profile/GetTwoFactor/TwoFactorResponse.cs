namespace SoccerTv.Api.Features.Profile.GetTwoFactor;

public record TwoFactorResponse(bool IsEnabled, int RecoveryCodesLeft, bool IsMachineRemembered);
