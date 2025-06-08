using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Images;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.Tasks;

namespace SimpleTube.RestApi.Commands.Internal.DownloadChannelImages;

internal sealed class DownloadChannelImagesCommandHandler
    : ICommandHandler<DownloadChannelImagesCommand, DownloadChannelImagesCommand.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IImageService _imageService;
    private readonly ITaskQueue<DownloadChannelImagesTask> _taskQueue;

    public DownloadChannelImagesCommandHandler(
        ConnectionStringProvider connectionStringProvider,
        IHttpClientFactory httpClientFactory,
        IImageService imageService,
        ITaskQueue<DownloadChannelImagesTask> taskQueue
    )
    {
        _connectionStringProvider = connectionStringProvider;
        _httpClientFactory = httpClientFactory;
        _imageService = imageService;
        _taskQueue = taskQueue;
    }

    public ValueTask<DownloadChannelImagesCommand.Result> Execute(
        DownloadChannelImagesCommand command,
        CancellationToken cancellationToken
    )
    {
        var task = new DownloadChannelImagesTask(
            command.ChannelId,
            _connectionStringProvider,
            _httpClientFactory,
            _imageService
        );
        _taskQueue.QueueTask(task);
        return ValueTask.FromResult(new DownloadChannelImagesCommand.Result());
    }
}
