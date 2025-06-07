using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.Mediator.Internal;
using TUnit.Assertions.AssertConditions.Throws;

namespace SimpleTube.RestApi.Tests.Unit.Infrastructure.Mediator.Internal;

public sealed class ValidatingDelegatingMessageHandlerTests
{
    [Test]
    public async Task Validator_ShouldBeExecuted_WhenValidatorIsRegistered(
        CancellationToken cancellationToken
    )
    {
        // Arrange.
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IValidator<Command>)).Returns(new Validator());

        // Act.
        var sut = new ValidatingDelegatingMessageHandler<Command, int>(serviceProvider)
        {
            Next = Substitute.For<DelegatingMessageHandler<Command, int>>(),
        };
        var invoke = async () =>
        {
            await sut.Invoke(new Command(), cancellationToken);
        };

        // Assert.
        await Assert.That(invoke).ThrowsExactly<ValidationException>();
    }

    internal sealed record Command : ICommand<int>
    {
        public string Id { get; init; } = "";
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
