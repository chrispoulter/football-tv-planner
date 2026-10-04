namespace FootballTvPlanner.Api.Features.Fixtures.GetChannels;

public record GetChannelsResponse(List<GetChannelsItem> Items);

public record GetChannelsItem(Guid Id, string Name);
