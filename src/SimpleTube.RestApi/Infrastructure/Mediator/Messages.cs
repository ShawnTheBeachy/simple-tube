namespace SimpleTube.RestApi.Infrastructure.Mediator;

public interface IMessage<TResult>;

public interface ICommand<TResult> : IMessage<TResult>;

public interface IQuery<TResult> : IMessage<TResult>;
