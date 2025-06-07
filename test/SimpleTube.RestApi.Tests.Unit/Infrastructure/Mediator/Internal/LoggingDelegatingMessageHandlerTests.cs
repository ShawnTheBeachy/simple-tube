using Microsoft.Extensions.Logging;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.Mediator.Internal;
using SimpleTube.RestApi.Tests.Unit.TestHelpers.Logging;

namespace SimpleTube.RestApi.Tests.Unit.Infrastructure.Mediator.Internal;

public sealed class LoggingDelegatingMessageHandlerTests
{
    [Test]
    public async Task Message_ShouldBeLogged_WhenExecutingCommand(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var logger = new TestableLogger<LoggingDelegatingMessageHandler<Command, int>>();

        // Act.
        var sut = new LoggingDelegatingMessageHandler<Command, int>(logger)
        {
            Next = Substitute.For<DelegatingMessageHandler<Command, int>>(),
        };
        _ = await sut.Invoke(new Command(), cancellationToken);

        // Assert.
        await Assert.That(logger).Logged(LogLevel.Debug, $"Executing {nameof(Command)} {{}}");
    }

    internal sealed record Command : ICommand<int>;
}
