namespace SimpleTube.BlazorApp.Providers;

public sealed record ServerUrlProvider
{
    public required string ServerUrl { get; init; }
}
