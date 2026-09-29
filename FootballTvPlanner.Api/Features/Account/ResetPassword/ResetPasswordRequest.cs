using FluentValidation;

namespace FootballTvPlanner.Api.Features.Account.ResetPassword;

public record ResetPasswordRequest(string Email, string Code, string NewPassword);

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}
