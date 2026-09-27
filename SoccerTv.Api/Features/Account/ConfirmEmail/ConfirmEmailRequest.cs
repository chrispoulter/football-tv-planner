using FluentValidation;

namespace SoccerTv.Api.Features.Account.ConfirmEmail;

public record ConfirmEmailRequest(Guid UserId, string Code, string? ChangedEmail);

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.ChangedEmail).EmailAddress().MaximumLength(256);
    }
}
