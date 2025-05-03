using Moq;
using Order.Application.Dto;
using Order.Application.Service;
using Order.Domain.Abstraction;
using Order.Domain.Entities;
using Shared.Kernel.Core.Enuns;
using System.Linq.Expressions;

namespace Order.Tests;

public class OrderServiceTest
{

    private readonly Mock<IOrderRepository> _unitOfWorkMock;
    private readonly OrderService _orderService;

    public OrderServiceTest()
    {
        _unitOfWorkMock = new Mock<IOrderRepository>();
        _orderService = new OrderService(_unitOfWorkMock.Object);
    }

    [Fact(DisplayName = "Test 01")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Add_ShouldReturnSuccessResult_WhenOrderIsAdded()
    {
        // Arrange
        var orderDto = new CreateOrderDto 
        {
            Items = new List<OrderItemDto> {
                new OrderItemDto{
                    Id = Guid.NewGuid(),
                    OrderId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 1,
                    TotalPrice = 100,
                    UnitPrice = 100
                }
            },
            CreatedAt = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            Status = (int)OrderStatus.Created,
            Name = "Test",
            PaymentToken = "Token",
            TotalAmount = 100
        };

        _unitOfWorkMock.Setup(u => u.AddAsync(It.IsAny<Domain.Entities.Order>())).Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.Add(orderDto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(result.Data);
    }

    [Fact(DisplayName = "Test 02")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Delete_ShouldReturnSuccessResult_WhenOrderIsDeleted()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Domain.Entities.Order { Id = orderId };
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.Delete(orderId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(result.Data);
    }

    [Fact(DisplayName = "Test 03")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Delete_ShouldReturnFailureResult_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>()))
            .ReturnsAsync((Domain.Entities.Order)null);

        // Act
        var result = await _orderService.Delete(orderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Order not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 04")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Delete_ShouldThrowException_WhenErrorOccurs()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.Delete(orderId));
    }

    [Fact(DisplayName = "Test 04")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Get_ShouldReturnSuccessResult_WhenOrderIsFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Domain.Entities.Order 
        {
            Id = orderId,
            Items = new List<OrderItem> {
                new OrderItem{ 
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product",
                    Quantity = 1,   
                    TotalPrice = 100,
                    UnitPrice = 100
                } 
            },
            CreatedDate = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            Status = (int)OrderStatus.Created,
            Name = "Test",
            PaymentToken = "Token",
            TotalAmount = 100
        };
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>(), It.IsAny<Expression<Func<Domain.Entities.Order, object>>>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.Get(orderId);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "Test 04")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Get_ShouldReturnFailureResult_WhenOrderNotFound()
    {
        // Arrange
        List<Domain.Entities.Order> order = null;

        var orderId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(uow => uow.GetAllAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>(),
                         It.IsAny<Expression<Func<Domain.Entities.Order, object>>[]>())).ReturnsAsync(order);

        // Act
        var result = await _orderService.Get(orderId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Order not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 05")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Get_ShouldThrowException_WhenErrorOccurs()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(uow => uow.GetAllAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>(),
                 It.IsAny<Expression<Func<Domain.Entities.Order, object>>[]>())).ThrowsAsync(new Exception("Database error"));


        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.Get(orderId));
    }

    [Fact(DisplayName = "Test 06")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task List_ShouldReturnSuccessResult_WhenOrdersAreFound()
    {
        // Arrange
        var orders = new List<Domain.Entities.Order> { new Domain.Entities.Order() };
        _unitOfWorkMock.Setup(u => u.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Domain.Entities.Order, object>>>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _orderService.List();

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
    }

    [Fact(DisplayName = "Test 06")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task List_ShouldReturnFailureResult_WhenOrdersNotFound()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Domain.Entities.Order, object>>>()))
            .ReturnsAsync((IEnumerable<Domain.Entities.Order>)null);

        // Act
        var result = await _orderService.List();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Orders not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 07")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task List_ShouldThrowException_WhenErrorOccurs()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Domain.Entities.Order, object>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.List());
    }

    [Fact(DisplayName = "Test 08")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Update_ShouldReturnSuccessResult_WhenOrderIsUpdated()
    {
        // Arrange
        var orderDto = new UpdateOrderDto { Id = Guid.NewGuid() };
        var order = new Domain.Entities.Order { Id = orderDto.Id };
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.Update(orderDto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(result.Data);
    }

    [Fact(DisplayName = "Test 09")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Update_ShouldReturnFailureResult_WhenOrderNotFound()
    {
        // Arrange
        var orderDto = new UpdateOrderDto { Id = Guid.NewGuid() };
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>()))
            .ReturnsAsync((Domain.Entities.Order)null);

        // Act
        var result = await _orderService.Update(orderDto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Order not found", result.Messages.FirstOrDefault());
    }

    [Fact(DisplayName = "Test 10")]
    [Trait("OrderService", "OrderServiceTest")]
    public async Task Update_ShouldThrowException_WhenErrorOccurs()
    {
        // Arrange
        var orderDto = new UpdateOrderDto { Id = Guid.NewGuid() };
        _unitOfWorkMock.Setup(u => u.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Order, bool>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _orderService.Update(orderDto));
    }
}