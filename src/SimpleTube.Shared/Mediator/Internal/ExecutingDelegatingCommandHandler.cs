using Microsoft.Extensions.DependencyInjection;

namespace SimpleTube.Shared.Mediator.Internal;

internal sealed class ExecutingDelegatingCommandHandler<TCommand, TResult>
    : DelegatingMessageHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly IServiceProvider _serviceProvider;

    public ExecutingDelegatingCommandHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async ValueTask<TResult> Invoke(
        TCommand command,
        CancellationToken cancellationToken
    ) =>
        await _serviceProvider
            .GetRequiredService<ICommandHandler<TCommand, TResult>>()
            .Execute(command, cancellationToken);
}
