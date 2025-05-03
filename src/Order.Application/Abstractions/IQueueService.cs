namespace Order.Order.Application.Abstractions;

public interface IQueueService
{
    Uri OrderCreatedMessage { get; }
    Uri OrderDeletedMessage { get; }
    Uri DeleteCartMessage { get; }
}
