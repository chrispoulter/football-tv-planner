using FluentValidation;

namespace SoccerTv.Api.Features.Account.LoginTwoFactor;

/// <summary>
/// Either a code from the authenticator app or one of the recovery codes.
/// </summary>
public record LoginTwoFactorRequest(
    string? Code,
    string? RecoveryCode,
    bool RememberMe,
    bool RememberMachine
);

public class LoginTwoFactorRequestValidator : AbstractValidator<LoginTwoFactorRequest>
{
    public LoginTwoFactorRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.RecoveryCode))
            .WithMessage("'Code' or 'Recovery Code' must not be empty.");
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.RecoveryCode).MaximumLength(20);
    }
}
