using Message.Broker.RabbitMq;
using Message.Broker.RabbitMq.Configurations;
using Messaging.Contracts.Events.Product;
using Microsoft.Extensions.Options;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Services;

[ExcludeFromCodeCoverage]
public class ProductService : IProductService
{
    private readonly RabbitMqConfig _rabbitMqOptions;

    public ProductService(IOptions<RabbitMqConfig> options)
    {
        _rabbitMqOptions = new RabbitMqConfig();
        _rabbitMqOptions = options.Value;
    }

    public async Task PublishProductAddedEvent(ProductAddedDomainEvent product)
    {
        try
        {
            var instance = RabbitMqSingleton.GetInstance(_rabbitMqOptions.Host);

            var productCreated = new ProductAddedEvent
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Active = product.Active,
                CreateAt = product.CreatAt,
                ImageUri = product.ImageUri
            };

            var messageEndpoint = await instance.Bus.GetSendEndpoint(new Uri($"queue:{_rabbitMqOptions.Prefix}{QueueConfig.ProductCreatedMessage}"));
            await messageEndpoint.Send(productCreated);
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
            var productDeleted = new ProductDeletedEvent
            {
                ProductId = product.ProductId,
            };

            var instance = RabbitMqSingleton.GetInstance(_rabbitMqOptions.Host);

            var messageEndpoint = await instance.Bus.GetSendEndpoint(new Uri($"queue:{_rabbitMqOptions.Prefix}{QueueConfig.ProductDeletedMessage}"));
            await messageEndpoint.Send(productDeleted);
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

            var instance = RabbitMqSingleton.GetInstance(_rabbitMqOptions.Host);

            var messageEndpoint = await instance.Bus.GetSendEndpoint(new Uri($"queue:{_rabbitMqOptions.Prefix}{QueueConfig.ProductUpdatedMessage}"));
            await messageEndpoint.Send(productUpdated);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send updated product to Queue");
            throw;
        }
    }
}
