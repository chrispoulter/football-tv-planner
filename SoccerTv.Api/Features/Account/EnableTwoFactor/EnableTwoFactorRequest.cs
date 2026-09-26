using FluentValidation;

namespace SoccerTv.Api.Features.Account.EnableTwoFactor;

public record EnableTwoFactorRequest(string Code);

public class EnableTwoFactorRequestValidator : AbstractValidator<EnableTwoFactorRequest>
{
    public EnableTwoFactorRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}
