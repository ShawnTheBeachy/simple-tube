namespace SimpleTube.RestApi.Rest.Entities;

internal sealed record VideoStream : RestEntity
{
    public string? Type { get; init; }
}
