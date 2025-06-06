namespace SimpleTube.Shared.Mediator;

public abstract class DelegatingMessageHandler<TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    internal DelegatingMessageHandler<TMessage, TResult>? Next { get; set; }

    public virtual async ValueTask<TResult> Invoke(
        TMessage message,
        CancellationToken cancellationToken
    )
    {
        if (Next is not null)
            return await Next.Invoke(message, cancellationToken);

        return default!;
    }
}
