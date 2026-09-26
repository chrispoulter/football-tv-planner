using FluentValidation;

namespace SoccerTv.Api.Features.Profile.EnableTwoFactor;

public record EnableTwoFactorRequest(string Code);

public class EnableTwoFactorRequestValidator : AbstractValidator<EnableTwoFactorRequest>
{
    public EnableTwoFactorRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}
