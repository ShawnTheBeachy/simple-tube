namespace SimpleTube.RestApi.Rest.Subscriptions;

internal sealed record SubscriptionRestEntity
{
    public string? ChannelHandle { get; init; }
    public string? ChannelId { get; init; }
    public string? ChannelName { get; init; }
    public string? ChannelThumbnail { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public DateTimeOffset? LastModifiedAt { get; init; }
}
