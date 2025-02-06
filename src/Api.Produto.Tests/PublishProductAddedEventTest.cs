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
    private readonly IProductService _productService;
    public PublishProductAddedEventTest()
    {
        var rabbitMqConfig = new RabbitMqConfig
        {
            Host = "amqp://localhost",
            User = "guest",
            Pass = "guest"
        };

        var rabbitMqOptionsMock = new Mock<IOptions<RabbitMqConfig>>();
        rabbitMqOptionsMock.Setup(o => o.Value).Returns(rabbitMqConfig);

        _productService = new ProductService(rabbitMqOptionsMock.Object);
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
}
