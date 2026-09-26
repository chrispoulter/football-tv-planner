using FluentValidation;

namespace SoccerTv.Api.Features.Account.ChangeEmail;

public record ChangeEmailRequest(string NewEmail);

public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail).NotEmpty().EmailAddress().MaximumLength(256);
    }
}
