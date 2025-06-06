using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SimpleTube.Shared.Mediator.Internal;

internal sealed class LoggingDelegatingMessageHandler<TMessage, TResult>
    : DelegatingMessageHandler<TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    private readonly ILogger _logger;

    public LoggingDelegatingMessageHandler(
        ILogger<LoggingDelegatingMessageHandler<TMessage, TResult>> logger
    )
    {
        _logger = logger;
    }

    public override async ValueTask<TResult> Invoke(
        TMessage message,
        CancellationToken cancellationToken
    )
    {
        _logger.Executing(
            typeof(TMessage).Name,
            JsonSerializer.Serialize(message, MediatorJsonSerializerContext.Default.Options)
        );

        try
        {
            var result = await base.Invoke(message, cancellationToken);
            _logger.Returned(
                typeof(TResult).Name,
                JsonSerializer.Serialize(result, MediatorJsonSerializerContext.Default.Options)
            );
            return result;
        }
        catch (Exception exception)
        {
            _logger.Failed(typeof(TMessage).Name, exception);
            throw;
        }
    }
}

internal static partial class Logging
{
    [LoggerMessage(LogLevel.Debug, "Executing {MessageType} {Message}")]
    public static partial void Executing(this ILogger logger, string messageType, string message);

    [LoggerMessage(LogLevel.Error, "Failed to execute {MessageType}")]
    public static partial void Failed(this ILogger logger, string messageType, Exception exception);

    [LoggerMessage(LogLevel.Debug, "Returned {ResultType} {Result}")]
    public static partial void Returned(this ILogger logger, string resultType, string result);
}
