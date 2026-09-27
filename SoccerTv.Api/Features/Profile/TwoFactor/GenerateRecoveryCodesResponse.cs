namespace SoccerTv.Api.Features.Profile.TwoFactor;

public record RecoveryCodesResponse(IEnumerable<string> RecoveryCodes)
{
    public const int Count = 10;
}
