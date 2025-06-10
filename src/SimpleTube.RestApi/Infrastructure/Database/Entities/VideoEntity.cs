namespace SimpleTube.RestApi.Infrastructure.Database.Entities;

public sealed class VideoEntity
{
    public required string ChannelId { get; init; }
    public required string Description { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string ETag { get; init; }
    public required string Id { get; init; }
    public required DateTime PublishedAt { get; init; }
    public required string Thumbnail { get; init; }
    public required string Title { get; init; }
}
