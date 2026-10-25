using FluentValidation;
using FootballTvPlanner.Data.Users;
using FootballTvPlanner.Server.Common.Validation;

namespace FootballTvPlanner.Server.Features.Account.Register;

public record RegisterRequest(string Name, string Email, string Password);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().PlainEmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
