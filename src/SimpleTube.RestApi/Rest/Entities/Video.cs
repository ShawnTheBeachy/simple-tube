namespace SimpleTube.RestApi.Rest.Entities;

internal sealed record Video : RestEntity
{
    public Channel? Channel { get; init; }
    public string? Description { get; init; }
    public string? EmbedUrl { get; init; }
    public string? Id { get; init; }
    public DateTime? PublishedAt { get; init; }
    public IReadOnlyList<VideoStream>? Streams { get; init; }
    public string? Thumbnail { get; init; }
    public string? Title { get; init; }
}
