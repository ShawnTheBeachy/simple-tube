namespace SimpleTube.RestApi.Rest.Channels;

internal sealed record Channel : RestEntity
{
    public string? Banner { get; init; }
    public string? Handle { get; init; }
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Thumbnail { get; init; }
    public int? UnwatchedVideos { get; init; }
}
