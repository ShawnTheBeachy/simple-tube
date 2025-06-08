using SimpleTube.RestApi.Commands.Internal.DownloadChannelImages;
using SimpleTube.RestApi.Commands.Internal.ScanChannel;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands.Internal;

internal static class DependencyInjection
{
    public static IServiceCollection AddCommands(this IServiceCollection services) =>
        services
            .AddTransient<
                ICommandHandler<DownloadChannelImagesCommand, DownloadChannelImagesCommand.Result>,
                DownloadChannelImagesCommandHandler
            >()
            .AddScoped<DownloadChannelImagesOnSubscribedConsumer>()
            .AddTransient<
                ICommandHandler<ScanChannelCommand, ScanChannelCommand.Result>,
                ScanChannelCommandHandler
            >()
            .AddScoped<ScanChannelOnSubscribedConsumer>()
            .AddTransient<
                ICommandHandler<SubscribeCommand, SubscribeCommand.Result>,
                SubscribeCommandHandler
            >()
            .AddTransient<
                ICommandHandler<UnsubscribeCommand, UnsubscribeCommand.Result>,
                UnsubscribeCommandHandler
            >();
}
