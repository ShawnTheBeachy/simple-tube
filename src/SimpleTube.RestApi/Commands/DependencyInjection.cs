using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Mediator;

namespace SimpleTube.RestApi.Commands;

internal static class DependencyInjection
{
    public static IServiceCollection AddCommands(this IServiceCollection services) =>
        services.AddTransient<
            ICommandHandler<SubscribeCommand, SubscribeCommand.Result>,
            SubscribeCommandHandler
        >();
}
