using FluentValidation;

namespace SoccerTv.Api.Features.Account.Login;

public record LoginRequest(string Email, string Password, bool RememberMe);

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
