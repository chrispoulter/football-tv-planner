using FluentValidation;

namespace FootballTvPlanner.Server.Features.Profile.SetPassword;

public record SetPasswordRequest(string NewPassword);

public class SetPasswordRequestValidator : AbstractValidator<SetPasswordRequest>
{
    public SetPasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
