using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

namespace SimpleTube.RestApi.Infrastructure.Images;

internal sealed class ImageService : IImageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<ImageOptions> _imageOptions;

    public ImageService(IHttpClientFactory httpClientFactory, IOptions<ImageOptions> imageOptions)
    {
        _httpClientFactory = httpClientFactory;
        _imageOptions = imageOptions;
    }

    private static void CreateIntermediateDirectories(string fileName)
    {
        var directory = new FileInfo(fileName).DirectoryName;
        Directory.CreateDirectory(directory!);
    }

    public async ValueTask<string> DownloadImage(
        string url,
        string? name,
        IReadOnlyList<string> nest,
        CancellationToken cancellationToken
    )
    {
        var imageStream = await _httpClientFactory
            .CreateClient()
            .GetStreamAsync(url, cancellationToken);

        var fileName =
            (
                string.IsNullOrWhiteSpace(name)
                    ? RandomStringGenerator.GenerateRandomString(20)
                    : $"{name}_{RandomStringGenerator.GenerateRandomString(5)}"
            ) + ".webp";
        var filePath = Path.Combine([_imageOptions.Value.Location, .. nest, fileName]);
        CreateIntermediateDirectories(filePath);
        using var image = await Image.LoadAsync(imageStream, cancellationToken);
        await image.SaveAsWebpAsync(filePath, cancellationToken);
        return Path.Combine([.. nest, fileName]).Replace('\\', '/');
    }

    public async ValueTask<string> DownloadImage(
        IReadOnlyList<string> urls,
        string? name,
        IReadOnlyList<string> nest,
        CancellationToken cancellationToken
    )
    {
        var exceptions = new List<Exception>();

        foreach (var url in urls)
        {
            try
            {
                var result = await DownloadImage(url, name, nest, cancellationToken);
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }

        if (exceptions.Count > 0)
            throw new AggregateException(exceptions);

        return "";
    }
}
