using System.Collections.Concurrent;
using SimpleTube.RestApi.Extensions;

namespace SimpleTube.RestApi.Infrastructure.Tasks.Internal;

internal sealed class TaskQueue<T> : ITaskQueue<T>, IAsyncDisposable
    where T : ITask
{
    private CancellationTokenSource? _cancellation;
    private readonly ConcurrentQueue<ITask> _queue = [];

    public async ValueTask DisposeAsync()
    {
        if (_cancellation is null)
            return;

        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }

    private async ValueTask Execute()
    {
        if (_cancellation is not null)
            return;

        _cancellation = new CancellationTokenSource();

        while (_queue.TryDequeue(out var task))
            await task.Execute(_cancellation.Token);

        _cancellation = null;
    }

    public void QueueTask(T task)
    {
        _queue.Enqueue(task);
        Execute().FireAndForget();
    }
}
