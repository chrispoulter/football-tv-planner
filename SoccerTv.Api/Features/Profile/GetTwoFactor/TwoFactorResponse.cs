namespace SoccerTv.Api.Features.Account.GetTwoFactor;

public record TwoFactorResponse(bool IsEnabled, int RecoveryCodesLeft, bool IsMachineRemembered);
