namespace SoccerTv.Api.Features.Account.Login;

/// <summary>
/// When two-factor authentication is required, complete the login with a code from the
/// authenticator app.
/// </summary>
public record LoginResponse(bool RequiresTwoFactor);
