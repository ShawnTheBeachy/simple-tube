namespace SimpleTube.RestApi.Infrastructure.Mediator;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    ValueTask<TResult> Execute(TCommand command, CancellationToken cancellationToken);
}

public interface IMultiCommandHandler<in TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    ValueTask<IReadOnlyList<TResult>> Execute(
        IReadOnlyList<TCommand> commands,
        CancellationToken cancellationToken
    );
}

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    ValueTask<TResult> Execute(TQuery query, CancellationToken cancellationToken);
}
