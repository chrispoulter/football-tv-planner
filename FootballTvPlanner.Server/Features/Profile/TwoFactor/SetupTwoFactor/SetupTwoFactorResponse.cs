namespace FootballTvPlanner.Server.Features.Profile.TwoFactor.SetupTwoFactor;

public record SetupTwoFactorResponse(string SharedKey, string AuthenticatorUri);
