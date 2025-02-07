namespace Product.Infrastructure.RabbitMq;

public interface IQueueService
{
    Uri ProductCreatedMessage { get; }
    Uri ProductUpdatedMessage { get; }
    Uri ProductDeletedMessage { get; }
}
