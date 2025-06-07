namespace SimpleTube.RestApi.Infrastructure.Tasks;

public interface ITaskQueue<in T>
    where T : ITask
{
    void QueueTask(T task);
}
