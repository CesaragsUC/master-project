using MassTransit;
using Messaging.Contracts.Events.Product;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Serilog;

namespace Product.Services;

public class ProductService : IProductService
{
    private readonly IPublishEndpoint _publish;
    public ProductService(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public async Task PublishProductAddedEvent(ProductAddedDomainEvent product)
    {
        try
        {
            await _publish.Publish<ProductAddedEvent>(new ProductAddedEvent
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Active = product.Active,
                CreateAt = product.CreatAt,
                ImageUri = product.ImageUri
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send saved product to Queue");
            throw;
        }

    }

    public async Task PublishProductDeletedEvent(ProductDeletedDomainEvent product)
    {
        try
        {
            await _publish.Publish<ProductDeletedEvent>(new ProductDeletedEvent
            {
                ProductId = product.ProductId,
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send deleted product to Queue");
            throw;
        }
    }

    public async Task PublishProductUpdatedEvent(ProductUpdatedDomainEvent product)
    {
        try
        {
            await _publish.Publish<ProductUpdatedEvent>(new ProductUpdatedEvent
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Active = product.Active,
                ImageUri = product.ImageUri
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send updated product to Queue");
            throw;
        }
    }
}
