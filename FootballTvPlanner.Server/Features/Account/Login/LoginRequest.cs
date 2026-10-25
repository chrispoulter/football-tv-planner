using FluentValidation;
using FootballTvPlanner.Server.Common.Validation;

namespace FootballTvPlanner.Server.Features.Account.Login;

public record LoginRequest(string Email, string Password, bool RememberMe);

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().PlainEmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
    }
}
