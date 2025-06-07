namespace SimpleTube.RestApi.Infrastructure.Mediator;

public interface IMediator
{
    ValueTask<TResult> Execute<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken
    )
        where TCommand : ICommand<TResult>;

    ValueTask<IReadOnlyList<TResult>> Execute<TCommand, TResult>(
        IReadOnlyList<TCommand> commands,
        CancellationToken cancellationToken
    )
        where TCommand : ICommand<TResult>;

    ValueTask<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>;
}
