using FluentValidation;

namespace FootballTvPlanner.Api.Features.Fixtures.GetFixtures;

/// <summary>
/// The client decides what a "day" is in the viewer's time zone and sends it as a UTC range.
/// </summary>
public record GetFixturesRequest(
    DateTimeOffset? From,
    DateTimeOffset? To,
    Guid? CompetitionId,
    Guid? ChannelId,
    bool? Bookmarked
);

public class GetFixturesRequestValidator : AbstractValidator<GetFixturesRequest>
{
    private static readonly TimeSpan MaxRange = TimeSpan.FromDays(7);

    public GetFixturesRequestValidator()
    {
        RuleFor(x => x.From).NotEmpty();
        RuleFor(x => x.To)
            .NotEmpty()
            .GreaterThan(x => x.From)
            .Must((request, to) => to - request.From <= MaxRange)
            .WithMessage("'To' must be no more than 7 days after 'From'.");
    }
}
