namespace SimpleTube.RestApi.Infrastructure.Database.Entities;

public sealed class SubscriptionEntity : AuditableEntity
{
    public required string ChannelHandle { get; init; }
    public required string ChannelId { get; init; }
    public required string ChannelName { get; init; }
    public required string ChannelThumbnail { get; init; }
}
