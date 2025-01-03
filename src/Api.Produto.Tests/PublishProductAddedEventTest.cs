using Bogus;
using MassTransit;
using Messaging.Contracts.Events.Product;
using Moq;
using Product.Domain.Abstractions;
using Product.Domain.Events;
using Product.Domain.Models;
using Product.Services;

namespace Product.Api.Tests;

public class PublishProductAddedEventTest
{
    private readonly Mock<IPublishEndpoint> _publish;

    private readonly IProductService _productService;
    public PublishProductAddedEventTest()
    {
        _publish = new Mock<IPublishEndpoint>();
        _productService = new ProductService(_publish.Object);
    }

    [Fact(DisplayName = "Teste 1 - Send product created to queue ")]
    [Trait("Product.Services", "Send Message")]
    public async Task Should_PublishProductCreatedToQueue_ReturnSuccess()
    {
        Faker faker = new Faker("pt_BR");

        var productCreated = new ProductAddedDomainEvent
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(),
            Active = faker.Random.Bool(),
            ImageUri = faker.Image.PicsumUrl(),
            CreatAt = DateTime.Now
        };

        _publish.Setup(x => x.Publish<ProductAddedEvent>(It.IsAny<ProductAddedEvent>(), default));

        await _productService.PublishProductAddedEvent(productCreated);

    }

    [Fact(DisplayName = "Teste 2 - Fail to Send product created to queue ")]
    [Trait("Product.Services", "Send Message")]
    public async Task Should_PublishProductCreatedToQueue_ReturnException()
    {
        Faker faker = new Faker("pt_BR");

        var productCreated = new ProductAddedDomainEvent
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(),
            Active = faker.Random.Bool(),
            ImageUri = faker.Image.PicsumUrl(),
            CreatAt = DateTime.Now
        };

        _publish
       .Setup(x => x.Publish(It.IsAny<ProductAddedEvent>(), default))
       .ThrowsAsync(new Exception("Error to send saved product to Queue"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _productService.PublishProductAddedEvent(productCreated));
        Assert.Equal("Error to send saved product to Queue", exception.Message);

    }


    [Fact(DisplayName = "Teste 3 - Send product deleted to queue ")]
    [Trait("Product.Services", "Send Message")]
    public async Task Should_PublishProductDeletedToQueue_ReturnSuccess()
    {
        Faker faker = new Faker("pt_BR");

        var productDeleted = new ProductDeletedDomainEvent
        {
            ProductId = Guid.NewGuid().ToString()
        };

        _publish.Setup(x => x.Publish<ProductDeletedEvent>(It.IsAny<ProductDeletedEvent>(), default));

        await _productService.PublishProductDeletedEvent(productDeleted);
    }

    [Fact(DisplayName = "Teste 3 - Fail to Send product deleted to queue ")]
    [Trait("Product.Services", "Send Message")]
    public async Task Should_PublishProductDeletedToQueue_ReturnException()
    {
        Faker faker = new Faker("pt_BR");

        var productDeleted = new ProductDeletedDomainEvent
        {
            ProductId = Guid.NewGuid().ToString()
        };


        _publish
        .Setup(x => x.Publish(It.IsAny<ProductDeletedEvent>(), default))
        .ThrowsAsync(new Exception("Error to send deleted product to Queue"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _productService.PublishProductDeletedEvent(productDeleted));
        Assert.Equal("Error to send deleted product to Queue", exception.Message);
    }

    [Fact(DisplayName = "Teste 4 - Send product updated to queue ")]
    [Trait("Product.Services", "Send Message")]
    public async Task Should_PublishProductUpdatedToQueue_ReturnSuccess()
    {
        Faker faker = new Faker("pt_BR");

        var productUpdated = new ProductUpdatedDomainEvent
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(),
            Active = faker.Random.Bool(),
            ImageUri = faker.Image.PicsumUrl(),

        };

        _publish.Setup(x => x.Publish<ProductUpdatedEvent>(It.IsAny<ProductUpdatedEvent>(), default));

        await _productService.PublishProductUpdatedEvent(productUpdated);
    }

    [Fact(DisplayName = "Teste 5 - Fail to Send product updated to queue ")]
    [Trait("Product.Services", "Send Message")]
    public async Task Should_PublishProductUpdatedToQueue_ReturnException()
    {
        Faker faker = new Faker("pt_BR");

        var productUpdated = new ProductUpdatedDomainEvent
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(),
            Active = faker.Random.Bool(),
            ImageUri = faker.Image.PicsumUrl(),

        };

        _publish
        .Setup(x => x.Publish(It.IsAny<ProductUpdatedEvent>(), default))
        .ThrowsAsync(new Exception("Error to send updated product to Queue"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _productService.PublishProductUpdatedEvent(productUpdated));
        Assert.Equal("Error to send updated product to Queue", exception.Message);
    }
}
