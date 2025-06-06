namespace SimpleTube.RestApi.Rest;

internal sealed class RestEntity<T>
    where T : class
{
    public required T Entity { get; init; }
    public required string Url { get; init; }
}
