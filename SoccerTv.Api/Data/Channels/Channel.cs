namespace SoccerTv.Api.Data.Channels;

public class Channel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public ChannelType Type { get; set; }

    public int SortOrder { get; set; }
}
