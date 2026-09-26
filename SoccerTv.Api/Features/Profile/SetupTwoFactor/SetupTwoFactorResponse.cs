namespace SoccerTv.Api.Features.Account.SetupTwoFactor;

/// <summary>
/// The key to add to an authenticator app, and the same as a URI to show as a QR code.
/// </summary>
public record SetupTwoFactorResponse(string SharedKey, string AuthenticatorUri);
