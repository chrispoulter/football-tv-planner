namespace SoccerTv.Api.Features.Profile;

/// <summary>
/// Only ever shown once, as just their hashes aren't stored.
/// </summary>
public record RecoveryCodesResponse(IEnumerable<string> RecoveryCodes)
{
    public const int Count = 10;
}
