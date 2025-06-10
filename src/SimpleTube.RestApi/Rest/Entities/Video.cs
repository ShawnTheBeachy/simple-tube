namespace SimpleTube.RestApi.Rest.Entities;

internal sealed record Video : RestEntity
{
    public string? Id { get; init; }
    public DateTime? PublishedAt { get; init; }
    public string? Thumbnail { get; init; }
    public string? Title { get; init; }
}
