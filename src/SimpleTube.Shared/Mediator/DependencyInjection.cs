using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleTube.Shared.Mediator.Internal;

namespace SimpleTube.Shared.Mediator;

public static class DependencyInjection
{
    /// <summary>
    /// Adds the IMediator implementation to the DI container along with default delegating handlers.
    /// <para>
    /// This method can be called multiple times safely.
    /// </para>
    /// <param name="configure">
    /// If <paramref name="configure">configure</paramref> is provided, you must call <c>.AddLogging()</c> and <c>.AddValidation()</c> yourself since it is assumed that you want to add some custom handlers, and we don't know what order you want everything in.
    /// </param>
    /// </summary>
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Action<MediatorConfigurationBuilder>? configure = null
    )
    {
        services.TryAddTransient<IMediator, Internal.Mediator>();
        var configBuilder = new MediatorConfigurationBuilder(services);

        if (configure is not null)
            configure(configBuilder);
        else
            configBuilder.AddLogging().AddValidation();

        return services;
    }

    public sealed class MediatorConfigurationBuilder
    {
        private readonly IServiceCollection _services;

        public MediatorConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public MediatorConfigurationBuilder AddDelegatingMessageHandler(
            Type handlerType,
            ServiceLifetime lifetime = ServiceLifetime.Transient
        )
        {
            _services.TryAddEnumerable(
                new ServiceDescriptor(typeof(DelegatingMessageHandler<,>), handlerType, lifetime)
            );
            return this;
        }

        public MediatorConfigurationBuilder AddDelegatingMessageHandler<
            TCommand,
            TResult,
            THandler
        >(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TCommand : ICommand<TResult>
            where THandler : DelegatingMessageHandler<TCommand, TResult>
        {
            _services.TryAddEnumerable(
                new ServiceDescriptor(
                    typeof(DelegatingMessageHandler<TCommand, TResult>),
                    typeof(THandler),
                    lifetime
                )
            );
            return this;
        }

        public MediatorConfigurationBuilder AddLogging()
        {
            _services.TryAddEnumerable(
                ServiceDescriptor.Transient(
                    typeof(DelegatingMessageHandler<,>),
                    typeof(LoggingDelegatingMessageHandler<,>)
                )
            );
            return this;
        }

        public MediatorConfigurationBuilder AddValidation()
        {
            _services.TryAddEnumerable(
                ServiceDescriptor.Transient(
                    typeof(DelegatingMessageHandler<,>),
                    typeof(ValidatingDelegatingMessageHandler<,>)
                )
            );
            return this;
        }
    }
}
