namespace Basket.Infrastructure.RabbitMq;

public interface IQueueService
{
    Uri OrderCheckoutEventMessage { get; }
    Uri OrderDeletedMessage { get; }
}
