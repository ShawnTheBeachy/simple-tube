namespace SimpleTube.RestApi.Infrastructure.Tasks;

public interface ITask
{
    ValueTask Execute(CancellationToken cancellationToken);
}
