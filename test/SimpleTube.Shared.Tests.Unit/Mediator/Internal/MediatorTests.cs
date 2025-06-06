using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimpleTube.Shared.Mediator;
using TUnit.Assertions.AssertConditions.Throws;
using Sut = SimpleTube.Shared.Mediator.Internal.Mediator;

namespace SimpleTube.Shared.Tests.Unit.Mediator.Internal;

public sealed class MediatorTests
{
    [Test]
    public async Task DelegatingHandlers_ShouldBeCalled_WhenCommandIsExecuted(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var services = new ServiceCollection()
            .AddMediator()
            .AddLogging()
            .AddTransient<IValidator<Query>, Query.FailingValidator>()
            .AddSingleton(Substitute.For<IQueryHandler<Query, string>>())
            .BuildServiceProvider();

        // Act.
        var sut = new Sut(services);
        var query = new Query { Value = "foo" };
        var execute = async () => await sut.Query<Query, string>(query, cancellationToken);

        // Assert.
        await Assert.That(execute).ThrowsExactly<ValidationException>();
    }

    [Test]
    public async Task DelegatingHandlers_ShouldBeCalled_WhenQueryIsExecuted(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var services = new ServiceCollection()
            .AddMediator()
            .AddLogging()
            .AddTransient<IValidator<Command>, Command.FailingValidator>()
            .AddSingleton(Substitute.For<ICommandHandler<Command, int>>())
            .BuildServiceProvider();

        // Act.
        var sut = new Sut(services);
        var command = new Command();
        var execute = async () => await sut.Execute<Command, int>(command, cancellationToken);

        // Assert.
        await Assert.That(execute).ThrowsExactly<ValidationException>();
    }

    [Test]
    public async Task DelegatingHandlers_ShouldBeCalledInTheOrderTheyWereRegistered(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var services = new ServiceCollection()
            .AddTransient<DelegatingMessageHandler<Command, int>, DelegatingHandlerB>()
            .AddTransient<DelegatingMessageHandler<Command, int>, DelegatingHandlerA>()
            .AddTransient<DelegatingMessageHandler<Command, int>, DelegatingHandlerC>()
            .AddSingleton(Substitute.For<ICommandHandler<Command, int>>())
            .BuildServiceProvider();

        // Act.
        var sut = new Sut(services);
        var command = new Command();
        _ = await sut.Execute<Command, int>(command, cancellationToken);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(command.InvokedHandlers).HasCount(3);
        await Assert.That(command.InvokedHandlers[0]).IsTypeOf<DelegatingHandlerB>();
        await Assert.That(command.InvokedHandlers[1]).IsTypeOf<DelegatingHandlerA>();
        await Assert.That(command.InvokedHandlers[2]).IsTypeOf<DelegatingHandlerC>();
    }

    [Test]
    public async Task Handler_ShouldOnlyBeCalledOnce_WhenMultipleCommandsAreExecutedAtOnce(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var handler = Substitute.For<IMultiCommandHandler<Command, int>>();
        var services = new ServiceCollection()
            .AddSingleton<ICommandHandler<Command, int>>(handler)
            .BuildServiceProvider();

        // Act.
        var sut = new Sut(services);
        _ = await sut.Execute<Command, int>(
            [new Command(), new Command(), new Command()],
            cancellationToken
        );

        // Assert.
        await handler
            .DidNotReceiveWithAnyArgs()
            .Execute(Arg.Any<Command>(), Arg.Any<CancellationToken>());
        await handler
            .Received(1)
            .Execute(
                Arg.Is<IReadOnlyList<Command>>(x => x.Count == 3),
                Arg.Any<CancellationToken>()
            );
    }

    [Test]
    public async Task MultipleResults_ShouldBeReturned_WhenHandlerIsMultiCommandHandler(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var handler = Substitute.For<IMultiCommandHandler<Command, int>>();
        handler
            .Execute(Arg.Any<IReadOnlyList<Command>>(), Arg.Any<CancellationToken>())
            .Returns([1, 2, 3]);
        var services = new ServiceCollection()
            .AddSingleton<ICommandHandler<Command, int>>(handler)
            .BuildServiceProvider();

        // Act.
        var sut = new Sut(services);
        var results = await sut.Execute<Command, int>(
            [new Command(), new Command(), new Command()],
            cancellationToken
        );

        // Assert.
        await handler
            .Received(1)
            .Execute(
                Arg.Is<IReadOnlyList<Command>>(x => x.Count == 3),
                Arg.Any<CancellationToken>()
            );
        await Assert.That(results).HasCount(3);
    }

    [Test]
    public async Task MultipleResults_ShouldBeReturned_WhenHandlerIsNotMultiCommandHandler(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var handler = Substitute.For<ICommandHandler<Command, int>>();
        handler.Execute(Arg.Any<Command>(), Arg.Any<CancellationToken>()).Returns(1);
        var services = new ServiceCollection().AddSingleton(handler).BuildServiceProvider();

        // Act.
        var sut = new Sut(services);
        var results = await sut.Execute<Command, int>(
            [new Command(), new Command(), new Command()],
            cancellationToken
        );

        // Assert.
        await handler.Received(3).Execute(Arg.Any<Command>(), Arg.Any<CancellationToken>());
        await Assert.That(results).HasCount(3);
    }

    [Test]
    public async Task Result_ShouldBeReturned_WhenQueryIsExecuted(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var handler = Substitute.For<IQueryHandler<Query, string>>();
        handler
            .Execute(Arg.Any<Query>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.ArgAt<Query>(0).Value);
        var services = new ServiceCollection().AddSingleton(handler).BuildServiceProvider();

        // Act.
        var query = new Query { Value = "foo" };
        var sut = new Sut(services);
        var result = await sut.Query<Query, string>(query, cancellationToken);

        // Assert.
        await handler
            .Received(1)
            .Execute(Arg.Is<Query>(x => x.Value == "foo"), Arg.Any<CancellationToken>());
        await Assert.That(result).IsEqualTo("foo");
    }

    internal sealed record Command : ICommand<int>
    {
        public List<DelegatingMessageHandler<Command, int>> InvokedHandlers { get; } = [];

        public sealed class FailingValidator : AbstractValidator<Command>
        {
            public FailingValidator()
            {
                RuleFor(x => x).Must(_ => false);
            }
        }
    }

    internal sealed record Query : IQuery<string>
    {
        public required string Value { get; init; }

        public sealed class FailingValidator : AbstractValidator<Query>
        {
            public FailingValidator()
            {
                RuleFor(x => x).Must(_ => false);
            }
        }
    }

    private abstract class DelegatingHandlerBase : DelegatingMessageHandler<Command, int>
    {
        public override async ValueTask<int> Invoke(
            Command command,
            CancellationToken cancellationToken
        )
        {
            command.InvokedHandlers.Add(this);
            return await base.Invoke(command, cancellationToken);
        }
    }

    private sealed class DelegatingHandlerA : DelegatingHandlerBase;

    private sealed class DelegatingHandlerB : DelegatingHandlerBase;

    private sealed class DelegatingHandlerC : DelegatingHandlerBase;
}
