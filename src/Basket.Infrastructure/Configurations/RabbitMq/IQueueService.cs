namespace Basket.Infrastructure.RabbitMq;

public interface IQueueService
{
    Uri OrderCreatedMessage { get; }
    Uri OrderDeletedMessage { get; }
}
