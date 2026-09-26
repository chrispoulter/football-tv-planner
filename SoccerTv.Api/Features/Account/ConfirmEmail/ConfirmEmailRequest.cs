using FluentValidation;

namespace SoccerTv.Api.Features.Account.ConfirmEmail;

/// <summary>
/// The values from the link in the confirmation email. The changed email is only included when
/// confirming a new email address.
/// </summary>
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
