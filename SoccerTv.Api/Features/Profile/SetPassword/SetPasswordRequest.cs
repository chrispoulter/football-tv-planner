using FluentValidation;

namespace SoccerTv.Api.Features.Profile.SetPassword;

public record SetPasswordRequest(string NewPassword);

public class SetPasswordRequestValidator : AbstractValidator<SetPasswordRequest>
{
    public SetPasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}
