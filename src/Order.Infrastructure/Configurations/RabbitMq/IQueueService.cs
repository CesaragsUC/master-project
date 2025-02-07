namespace Order.Infrastructure.RabbitMq;

public interface IQueueService
{
    Uri OrderCreatedMessage { get; }
    Uri OrderDeletedMessage { get; }
}
