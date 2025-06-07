using FluentValidation;

namespace SimpleTube.RestApi.Infrastructure.Mediator.Internal;

internal sealed class ValidatingDelegatingMessageHandler<TMessage, TResult>
    : DelegatingMessageHandler<TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    private readonly IServiceProvider _services;

    public ValidatingDelegatingMessageHandler(IServiceProvider services)
    {
        _services = services;
    }

    public override async ValueTask<TResult> Invoke(
        TMessage message,
        CancellationToken cancellationToken
    )
    {
        var validator = _services.GetService<IValidator<TMessage>>();

        if (validator is not null)
            await validator.ValidateAndThrowAsync(message, cancellationToken);

        return await base.Invoke(message, cancellationToken);
    }
}
