using SoccerTv.Api.Data.Channels;

namespace SoccerTv.Api.Data.Fixtures;

public class FixtureBroadcast
{
    public Guid FixtureId { get; set; }

    public Fixture Fixture { get; set; } = null!;

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; } = null!;
}
