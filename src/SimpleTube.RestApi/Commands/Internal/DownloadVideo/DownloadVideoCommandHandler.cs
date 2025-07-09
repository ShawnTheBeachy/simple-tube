using SimpleTube.RestApi.Infrastructure.Downloads;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.Tasks;

namespace SimpleTube.RestApi.Commands.Internal.DownloadVideo;

internal sealed class DownloadVideoCommandHandler
    : ICommandHandler<DownloadVideoCommand, DownloadVideoCommand.Result>
{
    private readonly IDownloadsManager _downloadsManager;
    private readonly ITaskQueue<DownloadVideoTask> _taskQueue;

    public DownloadVideoCommandHandler(
        IDownloadsManager downloadsManager,
        ITaskQueue<DownloadVideoTask> taskQueue
    )
    {
        _downloadsManager = downloadsManager;
        _taskQueue = taskQueue;
    }

    public ValueTask<DownloadVideoCommand.Result> Execute(
        DownloadVideoCommand command,
        CancellationToken cancellationToken
    )
    {
        _taskQueue.QueueTask(new DownloadVideoTask(command.VideoId, _downloadsManager));
        return ValueTask.FromResult(new DownloadVideoCommand.Result());
    }
}
