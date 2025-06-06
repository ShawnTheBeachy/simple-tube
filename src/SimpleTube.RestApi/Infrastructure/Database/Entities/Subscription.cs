namespace SimpleTube.RestApi.Infrastructure.Database.Entities;

internal sealed class Subscription : AuditableEntity
{
    public required string ChannelHandle { get; init; }
    public required string ChannelId { get; init; }
    public required string ChannelName { get; init; }
}
