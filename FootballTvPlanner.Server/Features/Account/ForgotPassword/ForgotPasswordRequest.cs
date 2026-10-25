using FluentValidation;
using FootballTvPlanner.Server.Common.Validation;

namespace FootballTvPlanner.Server.Features.Account.ForgotPassword;

public record ForgotPasswordRequest(string Email);

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().PlainEmailAddress().MaximumLength(254);
    }
}
