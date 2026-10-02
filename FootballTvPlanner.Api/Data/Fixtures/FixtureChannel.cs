using FootballTvPlanner.Api.Data.Channels;

namespace FootballTvPlanner.Api.Data.Fixtures;

public class FixtureChannel
{
    public Guid FixtureId { get; set; }

    public Fixture Fixture { get; set; } = null!;

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; } = null!;
}
