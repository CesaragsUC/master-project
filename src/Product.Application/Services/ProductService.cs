using Message.Broker.Abstractions;
using Messaging.Contracts.Events.Product;
using Product.Application.Abstractions;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Serilog;

namespace Product.Application.Services;

public class ProductService : IProductService
{
    private readonly IQueueService _queueService;
    private readonly IRabbitMqService _rabbitMqService;

    public ProductService(IQueueService queueService,
        IRabbitMqService rabbitMqService)
    {
        _queueService = queueService;
        _rabbitMqService = rabbitMqService;
    }

    public async Task PublishProductAddedEvent(ProductAddedDomainEvent product)
    {
        try
        {
            var productCreated = new ProductAddedEvent
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Active = product.Active,
                CreateAt = product.CreatAt,
                ImageUri = product.ImageUri
            };

            await _rabbitMqService.Send(productCreated, _queueService.ProductCreatedMessage);
            

            Log.Information($"Product sent to queue: {_queueService.ProductCreatedMessage}");

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send product to Queue");
            throw;
        }

    }

    public async Task PublishProductDeletedEvent(ProductDeletedDomainEvent product)
    {
        try
        {
            var productDeleted = new ProductDeletedEvent
            {
                ProductId = product.ProductId,
            };

            await _rabbitMqService.Send(productDeleted, _queueService.ProductDeletedMessage);
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
            var productUpdated = new ProductUpdatedEvent
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Active = product.Active,
                ImageUri = product.ImageUri
            };

            await _rabbitMqService.Send(productUpdated, _queueService.ProductUpdatedMessage);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send updated product to Queue");
            throw;
        }
    }
}
