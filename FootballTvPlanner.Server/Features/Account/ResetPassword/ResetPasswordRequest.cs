using FluentValidation;
using FootballTvPlanner.Server.Common.Validation;

namespace FootballTvPlanner.Server.Features.Account.ResetPassword;

public record ResetPasswordRequest(string Email, string Code, string NewPassword);

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().PlainEmailAddress().MaximumLength(254);
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
