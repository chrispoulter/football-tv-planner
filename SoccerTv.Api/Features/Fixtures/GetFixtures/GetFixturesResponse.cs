namespace SoccerTv.Api.Features.Fixtures.GetFixtures;

public record GetFixturesResponse(DateOnly Date, List<FixtureSummary> Items);
