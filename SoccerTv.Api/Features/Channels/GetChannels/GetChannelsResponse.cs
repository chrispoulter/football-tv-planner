using SoccerTv.Api.Data.Channels;

namespace SoccerTv.Api.Features.Channels.GetChannels;

public record GetChannelsResponse(Guid Id, string Name, string Provider, ChannelType Type);
