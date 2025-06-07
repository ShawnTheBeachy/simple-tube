namespace SimpleTube.RestApi.Infrastructure.Database.Entities;

public sealed class ChannelEntity : AuditableEntity
{
    public string? Banner { get; init; }
    public required string Handle { get; init; }
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
}
