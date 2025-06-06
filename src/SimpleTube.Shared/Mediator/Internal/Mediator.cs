using Microsoft.Extensions.DependencyInjection;

namespace SimpleTube.Shared.Mediator.Internal;

internal sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private DelegatingMessageHandler<TMessage, TResult> BuildHandlers<TMessage, TResult>(
        DelegatingMessageHandler<TMessage, TResult> executingHandler
    )
        where TMessage : IMessage<TResult>
    {
        var delegatingHandlers = _serviceProvider
            .GetServices<DelegatingMessageHandler<TMessage, TResult>>()
            .Append(executingHandler)
            .ToArray();

        for (var i = 0; i < delegatingHandlers.Length; i++)
        {
            if (i < delegatingHandlers.Length - 1)
                delegatingHandlers[i].Next = delegatingHandlers[i + 1];
        }

        return delegatingHandlers[0];
    }

    public async ValueTask<TResult> Execute<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken
    )
        where TCommand : ICommand<TResult> =>
        await BuildHandlers(
                new ExecutingDelegatingCommandHandler<TCommand, TResult>(_serviceProvider)
            )
            .Invoke(command, cancellationToken);

    public async ValueTask<IReadOnlyList<TResult>> Execute<TCommand, TResult>(
        IReadOnlyList<TCommand> commands,
        CancellationToken cancellationToken
    )
        where TCommand : ICommand<TResult>
    {
        var executingHandler = new MultiExecutingDelegatingCommandHandler<TCommand, TResult>(
            _serviceProvider
        );
        var handler = BuildHandlers(executingHandler);

        foreach (var command in commands)
            _ = await handler.Invoke(command, cancellationToken);

        return await executingHandler.Invoke(cancellationToken);
    }

    public async ValueTask<TResult> Query<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken
    )
        where TQuery : IQuery<TResult> =>
        await BuildHandlers(new ExecutingDelegatingQueryHandler<TQuery, TResult>(_serviceProvider))
            .Invoke(query, cancellationToken);
}
