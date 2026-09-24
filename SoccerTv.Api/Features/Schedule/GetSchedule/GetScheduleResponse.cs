using SoccerTv.Api.Features.Fixtures;

namespace SoccerTv.Api.Features.Schedule.GetSchedule;

public record GetScheduleResponse(List<FixtureSummary> Items);
