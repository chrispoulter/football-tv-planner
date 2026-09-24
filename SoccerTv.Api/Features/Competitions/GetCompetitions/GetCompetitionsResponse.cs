namespace SoccerTv.Api.Features.Competitions.GetCompetitions;

public record GetCompetitionsResponse(Guid Id, string Name, string ShortName, string Country);
