namespace SimpleTube.RestApi.Rest.Channels;

internal sealed record Channel : RestEntity
{
    public string? ChannelHandle { get; init; }
    public string? ChannelId { get; init; }
    public string? ChannelName { get; init; }
    public string? ChannelThumbnail { get; init; }
}
