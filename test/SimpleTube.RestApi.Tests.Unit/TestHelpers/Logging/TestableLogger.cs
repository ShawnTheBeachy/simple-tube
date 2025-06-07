using Microsoft.Extensions.Logging;

namespace SimpleTube.RestApi.Tests.Unit.TestHelpers.Logging;

internal sealed class TestableLogger<T> : ILogger<T>
{
    private readonly List<(LogLevel Level, EventId EventId, string Message)> _logs = [];

    public IReadOnlyCollection<(LogLevel Level, EventId EventId, string Message)> GetLogs() =>
        _logs.AsReadOnly();

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    ) => _logs.Add((logLevel, eventId, formatter(state, null)));

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => new EmptyDisposable();

    public bool Received(LogLevel level, EventId eventId, string message) =>
        _logs.Contains((level, eventId, message));

    public bool Received(LogLevel level, string message) =>
        _logs.Any(log =>
            log.Level == level
            && (
                message.EndsWith('*') ? log.Message.StartsWith(message[..^1])
                : message.StartsWith('*') ? log.Message.EndsWith(message[1..])
                : log.Message == message
            )
        );

    private sealed class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }
}
