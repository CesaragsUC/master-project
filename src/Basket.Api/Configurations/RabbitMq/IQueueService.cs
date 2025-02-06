namespace Basket.Api.RabbitMq;

public interface IQueueService
{
    Uri OrderCreatedMessage { get; }
    Uri OrderDeletedMessage { get; }
}
