using FluentValidation;

namespace FootballTvPlanner.Server.Features.Profile.TwoFactor.EnableTwoFactor;

public record EnableTwoFactorRequest(string Code);

public class EnableTwoFactorRequestValidator : AbstractValidator<EnableTwoFactorRequest>
{
    public EnableTwoFactorRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}
