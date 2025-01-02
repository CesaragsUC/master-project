using Product.Domain.Events;

namespace Product.Domain.Abstractions;

public interface IProductService
{
    Task PublishProductAddedEvent(ProductAddedDomainEvent product);

    Task PublishProductUpdatedEvent(ProductUpdatedDomainEvent product);

    Task PublishProductDeletedEvent(ProductDeletedDomainEvent product);
}

