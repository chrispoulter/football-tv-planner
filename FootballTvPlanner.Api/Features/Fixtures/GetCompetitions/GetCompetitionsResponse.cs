namespace FootballTvPlanner.Api.Features.Fixtures.GetCompetitions;

public record GetCompetitionsResponse(List<GetCompetitionsItem> Items);

public record GetCompetitionsItem(Guid Id, string Name);
