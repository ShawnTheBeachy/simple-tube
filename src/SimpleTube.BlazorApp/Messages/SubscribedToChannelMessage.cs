namespace SimpleTube.BlazorApp.Messages;

internal sealed record SubscribedToChannelMessage
{
    public required string Handle { get; init; }
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Url { get; init; }
}
