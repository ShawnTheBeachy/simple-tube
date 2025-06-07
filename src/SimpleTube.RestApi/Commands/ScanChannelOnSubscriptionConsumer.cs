using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Messages;
using SlimMessageBus;

namespace SimpleTube.RestApi.Commands;

internal sealed class ScanChannelOnSubscriptionConsumer : IConsumer<SubscribedToChannelMessage>
{
    private readonly IMediator _mediator;

    public ScanChannelOnSubscriptionConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnHandle(
        SubscribedToChannelMessage message,
        CancellationToken cancellationToken
    ) =>
        await _mediator.Execute<ScanChannelCommand, ScanChannelCommand.Result>(
            new ScanChannelCommand { ChannelId = message.ChannelId },
            cancellationToken
        );
}
