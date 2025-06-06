using Microsoft.Extensions.DependencyInjection;

namespace SimpleTube.Shared.Mediator.Internal;

internal sealed class MultiExecutingDelegatingCommandHandler<TCommand, TResult>
    : DelegatingMessageHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _commandHandler;
    private readonly List<TCommand> _commands = [];
    private readonly List<TResult> _results = [];

    public MultiExecutingDelegatingCommandHandler(IServiceProvider serviceProvider)
    {
        _commandHandler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
    }

    public async ValueTask<IReadOnlyList<TResult>> Invoke(CancellationToken cancellationToken)
    {
        IReadOnlyList<TResult> results;

        if (_commandHandler is not IMultiCommandHandler<TCommand, TResult> multiCommandHandler)
            results = _results.ToArray();
        else
            results =
                _commands.Count < 1
                    ? []
                    : await multiCommandHandler.Execute(_commands.ToArray(), cancellationToken);

        _commands.Clear();
        _results.Clear();
        return results;
    }

    public override async ValueTask<TResult> Invoke(
        TCommand command,
        CancellationToken cancellationToken
    )
    {
        if (_commandHandler is not IMultiCommandHandler<TCommand, TResult>)
            _results.Add(await _commandHandler.Execute(command, cancellationToken));
        else
            _commands.Add(command);

        return default!;
    }
}
