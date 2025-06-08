namespace SimpleTube.RestApi.Infrastructure.Images;

public interface IImageService
{
    ValueTask<string> DownloadImage(
        string url,
        string? name,
        IReadOnlyList<string> nest,
        CancellationToken cancellationToken
    );
    ValueTask<string> DownloadImage(
        IReadOnlyList<string> urls,
        string? name,
        IReadOnlyList<string> nest,
        CancellationToken cancellationToken
    );
}
