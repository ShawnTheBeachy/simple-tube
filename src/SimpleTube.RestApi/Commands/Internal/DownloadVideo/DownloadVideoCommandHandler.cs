using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.Tasks;

namespace SimpleTube.RestApi.Commands.Internal.DownloadVideo;

internal sealed class DownloadVideoCommandHandler
    : ICommandHandler<DownloadVideoCommand, DownloadVideoCommand.Result>
{
    private readonly ITaskQueue<DownloadVideoTask> _taskQueue;

    public DownloadVideoCommandHandler(ITaskQueue<DownloadVideoTask> taskQueue)
    {
        _taskQueue = taskQueue;
    }

    public ValueTask<DownloadVideoCommand.Result> Execute(
        DownloadVideoCommand command,
        CancellationToken cancellationToken
    )
    {
        _taskQueue.QueueTask(new DownloadVideoTask(command.VideoId));
        return ValueTask.FromResult(new DownloadVideoCommand.Result());
    }
}
