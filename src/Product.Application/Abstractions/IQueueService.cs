namespace Product.Application.Abstractions;

public interface IQueueService
{
    Uri ProductCreatedMessage { get; }
    Uri ProductUpdatedMessage { get; }
    Uri ProductDeletedMessage { get; }
}
