namespace SimpleTube.RestApi.Infrastructure.Mediator.Internal;

internal sealed class ExecutingDelegatingQueryHandler<TQuery, TResult>
    : DelegatingMessageHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IServiceProvider _serviceProvider;

    public ExecutingDelegatingQueryHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async ValueTask<TResult> Invoke(
        TQuery query,
        CancellationToken cancellationToken
    ) =>
        await _serviceProvider
            .GetRequiredService<IQueryHandler<TQuery, TResult>>()
            .Execute(query, cancellationToken);
}
