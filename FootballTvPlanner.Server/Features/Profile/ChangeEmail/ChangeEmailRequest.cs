using FluentValidation;

namespace FootballTvPlanner.Server.Features.Profile.ChangeEmail;

public record ChangeEmailRequest(string NewEmail);

public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail).NotEmpty().EmailAddress().MaximumLength(256);
    }
}
