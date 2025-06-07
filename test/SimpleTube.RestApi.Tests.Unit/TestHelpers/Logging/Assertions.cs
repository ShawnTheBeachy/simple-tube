using System.Runtime.CompilerServices;
using TUnit.Assertions.AssertConditions;
using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SimpleTube.RestApi.Tests.Unit.TestHelpers.Logging;

internal static class AssertionExtensions
{
    public static InvokableValueAssertionBuilder<TestableLogger<T>> Logged<T>(
        this IValueSource<TestableLogger<T>> valueSource,
        LogLevel logLevel,
        string message,
        [CallerArgumentExpression(nameof(logLevel))] string doNotPopulateThisValue1 = "",
        [CallerArgumentExpression(nameof(message))] string doNotPopulateThisValue2 = ""
    ) =>
        valueSource.RegisterAssertion(
            assertCondition: new Assertions<T>(logLevel, message),
            argumentExpressions: [doNotPopulateThisValue1, doNotPopulateThisValue2]
        );
}

internal sealed class Assertions<T> : ValueAssertCondition<TestableLogger<T>>
{
    private readonly LogLevel _logLevel;
    private readonly string _message;

    internal Assertions(LogLevel logLevel, string message)
    {
        _logLevel = logLevel;
        _message = message;
    }

    protected override AssertionResult Passes(TestableLogger<T>? logger)
    {
        var received = logger?.Received(_logLevel, _message) ?? false;
        return AssertionResult.FailIf(
            !received,
            $"""
            it did not.

            Received {logger!.GetLogs().Count} non-matching logs:

            {string.Join("\n\n", logger.GetLogs().Select(x => $"[{x.Level}] {x.Message}"))}
            """
        );
    }

    protected override string GetFailureMessage(TestableLogger<T>? actualValue) =>
        $"Expected logger to log {_logLevel}: {_message}";
}
