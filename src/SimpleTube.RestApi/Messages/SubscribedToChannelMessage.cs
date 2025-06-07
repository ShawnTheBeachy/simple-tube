namespace SimpleTube.RestApi.Messages;

public sealed record SubscribedToChannelMessage
{
    public required string ChannelId { get; init; }
}
