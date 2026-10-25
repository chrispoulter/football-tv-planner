using FluentValidation;

namespace FootballTvPlanner.Server.Features.Account.LoginTwoFactor;

public record LoginTwoFactorRequest(string? Code, string? RecoveryCode, bool RememberMe);

public class LoginTwoFactorRequestValidator : AbstractValidator<LoginTwoFactorRequest>
{
    public LoginTwoFactorRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.RecoveryCode))
            .WithMessage("'Code' or 'Recovery Code' must not be empty.");
        RuleFor(x => x.Code).Length(6).When(x => !string.IsNullOrEmpty(x.Code));
        RuleFor(x => x.RecoveryCode).MaximumLength(20);
    }
}
