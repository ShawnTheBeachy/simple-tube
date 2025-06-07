using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

internal static class DependencyInjection
{
    public static IServiceCollection AddCommands(this IServiceCollection services) =>
        services
            .AddTransient<
                ICommandHandler<ScanChannelCommand, ScanChannelCommand.Result>,
                ScanChannelCommandHandler
            >()
            .AddScoped<ScanChannelOnSubscriptionConsumer>()
            .AddTransient<
                ICommandHandler<SubscribeCommand, SubscribeCommand.Result>,
                SubscribeCommandHandler
            >()
            .AddTransient<
                ICommandHandler<UnsubscribeCommand, UnsubscribeCommand.Result>,
                UnsubscribeCommandHandler
            >();
}
