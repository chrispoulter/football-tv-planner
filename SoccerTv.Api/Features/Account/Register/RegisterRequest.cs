using FluentValidation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.Register;

public record RegisterRequest(string Name, string Email, string Password);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(UserConfiguration.NameMaxLength);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}
