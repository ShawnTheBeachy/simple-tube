using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.Tasks;

namespace SimpleTube.RestApi.Commands.Internal.ScanChannel;

internal sealed class ScanChannelCommandHandler
    : ICommandHandler<ScanChannelCommand, ScanChannelCommand.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITaskQueue<ScanChannelTask> _taskQueue;

    public ScanChannelCommandHandler(
        ConnectionStringProvider connectionStringProvider,
        IHttpClientFactory httpClientFactory,
        ITaskQueue<ScanChannelTask> taskQueue
    )
    {
        _connectionStringProvider = connectionStringProvider;
        _httpClientFactory = httpClientFactory;
        _taskQueue = taskQueue;
    }

    public ValueTask<ScanChannelCommand.Result> Execute(
        ScanChannelCommand command,
        CancellationToken cancellationToken
    )
    {
        _taskQueue.QueueTask(
            new ScanChannelTask(command.ChannelId, _connectionStringProvider, _httpClientFactory)
        );
        return ValueTask.FromResult(new ScanChannelCommand.Result());
    }
}
