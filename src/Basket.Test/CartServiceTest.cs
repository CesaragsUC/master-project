using Basket.Api.Abstractions;
using Basket.Api.Dtos;
using Basket.Api.RabbitMq;
using Basket.Api.Services;
using Basket.Domain.Abstractions;
using Basket.Domain.Entities;
using Message.Broker.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Basket.Test;

public class CartServiceTest
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CartService _cartService;
    private readonly Mock<IDiscountApi> _discountApiMock;
    private readonly Mock<IQueueService> _queueServiceMock;
    private readonly Mock<IRabbitMqService> _rabbitmqServiceMock;

    public CartServiceTest()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _discountApiMock = new Mock<IDiscountApi>();
        _queueServiceMock = new Mock<IQueueService>();
        _rabbitmqServiceMock = new Mock<IRabbitMqService>();

        _cartService = new CartService(_cartRepositoryMock.Object,
            _cacheServiceMock.Object,
            _discountApiMock.Object,
            _queueServiceMock.Object,
            _rabbitmqServiceMock.Object);
    }


    [Fact]
    [Trait("Basket.Services", "Cache Redis")]
    public async Task GetCartAsync_ShouldReturnSuccess_WhenCartExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var cart = new Cart { CustomerId = customerId };

        _cacheServiceMock.Setup(x => x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Cart>>>(), It.IsAny<DistributedCacheEntryOptions>(), null))
            .ReturnsAsync(cart);

        // Act
        var result = await _cartService.GetCartAsync(customerId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(cart, result.Data);
    }

    [Fact]
    [Trait("Basket.Services", "Cache Redis")]
    public async Task GetCartAsync_ShouldReturnFailure_WhenCartDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _cacheServiceMock.Setup(x => x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Cart>>>(), It.IsAny<DistributedCacheEntryOptions>(), null))
            .ReturnsAsync((Cart)null);

        // Act
        var result = await _cartService.GetCartAsync(customerId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Data);
    }

    [Fact]
    [Trait("Basket.Services", "Cache Redis")]
    public async Task SaveOrUpdateCartAsync_ShouldReturnSuccess_WhenCartIsSaved()
    {
        // Arrange
        var cartDto = new CartDto { CustomerId = Guid.NewGuid() };

        var cart = new Cart { CustomerId = cartDto.CustomerId };

        //_mapperMock.Setup(x => x.Map<Cart>(cartDto)).Returns(cart);

        _cacheServiceMock.Setup(x => x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Cart>>>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<Func<Cart, Task>>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _cartService.SaveOrUpdateCartAsync(cartDto);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    [Trait("Basket.Services", "Cache Redis")]
    public async Task SaveOrUpdateCartAsync_ShouldReturnFailure_WhenCartIsNotSaved()
    {
        // Arrange
        var cartDto = new CartDto { CustomerId = Guid.NewGuid() };

        var cart = new Cart { CustomerId = cartDto.CustomerId };

        //_mapperMock.Setup(x => x.Map<Cart>(cartDto)).Returns(cart);

        _cacheServiceMock.Setup(x => x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Cart>>>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<Func<Cart, Task>>()))
            .ReturnsAsync((Cart)null);

        // Act
        var result = await _cartService.SaveOrUpdateCartAsync(cartDto);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    [Trait("Basket.Services", "Cache Redis")]
    public async Task GetCartAsync_ShouldLogErrorAndThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var exception = new Exception("Test exception");
        _cacheServiceMock.Setup(x => x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Cart>>>(), It.IsAny<DistributedCacheEntryOptions>(), null))
                         .ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _cartService.GetCartAsync(customerId));
        Assert.Equal("Test exception", ex.Message);
        _cacheServiceMock.Verify(x => x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Cart>>>(), It.IsAny<DistributedCacheEntryOptions>(), null), Times.Once);
    }
}
