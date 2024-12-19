using MassTransit;

namespace Casoft.MessageBus;

public interface IMessageBus : IConsumer
{
    Task PublishAsync<T>(T message);
}
