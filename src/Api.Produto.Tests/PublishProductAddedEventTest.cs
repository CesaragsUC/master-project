using Bogus;
using Message.Broker.RabbitMq;
using Microsoft.Extensions.Options;
using Moq;
using Product.Application.Services;
using Product.Domain.Abstractions;
using Product.Domain.Events;

namespace Product.Api.Tests;

public class PublishProductAddedEventTest
{
    private readonly Mock<IOptions<RabbitMqConfig>> _rabbitMqOptions;

    private readonly IProductService _productService;
    public PublishProductAddedEventTest()
    {
        _rabbitMqOptions = new Mock<IOptions<RabbitMqConfig>>();
        _productService = new ProductService(_rabbitMqOptions.Object);
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

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _productService.PublishProductUpdatedEvent(productUpdated));
        Assert.Equal("Error to send updated product to Queue", exception.Message);
    }
}
