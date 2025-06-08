using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Messages;
using SlimMessageBus;

namespace SimpleTube.RestApi.Commands.Internal.DownloadChannelImages;

internal sealed class DownloadChannelImagesOnSubscribedConsumer
    : IConsumer<SubscribedToChannelMessage>
{
    private readonly IMediator _mediator;

    public DownloadChannelImagesOnSubscribedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnHandle(
        SubscribedToChannelMessage message,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Execute<DownloadChannelImagesCommand, DownloadChannelImagesCommand.Result>(
            new DownloadChannelImagesCommand { ChannelId = message.ChannelId },
            cancellationToken
        );
    }
}
