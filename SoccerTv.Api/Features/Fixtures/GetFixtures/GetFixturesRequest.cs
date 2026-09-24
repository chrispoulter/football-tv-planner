using FluentValidation;

namespace SoccerTv.Api.Features.Fixtures.GetFixtures;

public record GetFixturesRequest(
    DateOnly? Date,
    Guid? CompetitionId,
    string? Provider,
    bool? Bookmarked
);

public class GetFixturesRequestValidator : AbstractValidator<GetFixturesRequest>
{
    public GetFixturesRequestValidator()
    {
        RuleFor(x => x.Provider).MaximumLength(50);
    }
}
