using AutoMapper;
using AutoMapper.QueryableExtensions;
using Billing.Application.Abstractions;
using Billing.Application.Dtos;
using Billing.Application.Service;
using Billing.Domain.Entities;
using Billing.Infrastructure;
using Billing.Infrastructure.Configurations.RabbitMq;
using HybridRepoNet.Abstractions;
using HybridRepoNet.Repository;
using Message.Broker.Abstractions;
using Moq;
using Refit;
using ResultNet;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;
using System.Linq.Expressions;
using System.Net;

namespace Billing.Tests;

public class PaymentServiceTest
{

    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork<BillingContext>> _unitOfWorkMock;
    private readonly Mock<IQueueService> _queueServiceMock;
    private readonly Mock<IOderApi> _oderApiMock;
    private readonly Mock<IRabbitMqService> _rabbitMqServiceMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork<BillingContext>>();
        _queueServiceMock = new Mock<IQueueService>();
        _oderApiMock = new Mock<IOderApi>();
        _rabbitMqServiceMock = new Mock<IRabbitMqService>();
        _mapperMock = new Mock<IMapper>();

        _paymentService = new PaymentService(
            _unitOfWorkMock.Object,
            _queueServiceMock.Object,
            _oderApiMock.Object,
            _rabbitMqServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact(DisplayName = "Test 01")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task CreatePaymentAsync_ShouldReturnSuccess_WhenOrderIsFound()
    {
        // Arrange
        var paymentDto = new PaymentCreatDto {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
             CreditCard = new CreditCardDto
             {
                 Holder = "John Doe",
                 CardNumber = "1234567890123456",
                 ExpirationDate = "12/2022",
                 SecurityCode = "123"
             }
        };

        var order = new OrderDto { 
            Id = paymentDto.OrderId,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 100,
            PaymentToken = PaymentTokenService.GenerateToken()
        };

        var orderResult = await Result<OrderDto>.SuccessAsync(order);

        var apiResponse = new ApiResponse<Result<OrderDto>>(
            new HttpResponseMessage(HttpStatusCode.OK),
            orderResult,
            new RefitSettings()
        );

        _oderApiMock.Setup(x => x.GetOrderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(apiResponse);

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().AddAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);

        _queueServiceMock.Setup(x => x.PaymentCreatedMessage).Returns(new Uri("queue:PaymentCreated"));

        // Act
        var result = await _paymentService.CreatePaymentAsync(paymentDto);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "Test 02")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task CreatePaymentAsync_ShouldThrowException_WhenTrySavePayment()
    {
        // Arrange
        // Arrange
        var paymentDto = new PaymentCreatDto { OrderId = Guid.NewGuid() };
        var order = new OrderDto { Id = paymentDto.OrderId, TotalAmount = 100 };

        var orderResult = await Result<OrderDto>.SuccessAsync(order);

        var apiResponse = new ApiResponse<Result<OrderDto>>(
            new HttpResponseMessage(HttpStatusCode.OK),
            orderResult,
            new RefitSettings()
        );

        _oderApiMock.Setup(x => x.GetOrderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(apiResponse);

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().AddAsync(It.IsAny<Payment>())).
            ThrowsAsync(new Exception("Error on create payment"));

        _queueServiceMock.Setup(x => x.PaymentCreatedMessage).Returns(new Uri("queue:PaymentCreated"));

        var exeption = await Assert.ThrowsAsync<Exception>(async () => await _paymentService.CreatePaymentAsync(paymentDto));
        Assert.Equal("Error on create payment", exeption.Message);
    }

    [Fact(DisplayName = "Test 03")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task DeletePaymentAsync_ShouldReturnSuccess_WhenPaymentIsDeleted()
    {
        // Arrange
        var transactionId = "PAY-20250208224655442-f8a439d460ec4609ae77757e7f1fc22e";

        _unitOfWorkMock.Setup(uow => uow.Repository<Payment>().SoftDeleteAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _paymentService.DeletePaymentAsync(transactionId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Payment deleted", result!.Messages!.First());

        _unitOfWorkMock.Verify(uow => uow.Repository<Payment>()
        .SoftDeleteAsync(It.IsAny<Expression<Func<Payment, bool>>>()), Times.Once);
    }

    [Fact(DisplayName = "Test 04")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task DeletePaymentAsync_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var transactionId = "PAY-20250208224655442-f8a439d460ec4609ae77757e7f1fc22e";
        var exception = new Exception("Error on delete payment");

        _unitOfWorkMock.Setup(uow => uow.Repository<Payment>().SoftDeleteAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(async () => await _paymentService.DeletePaymentAsync(transactionId));
        Assert.Equal(exception.Message, ex.Message);

        _unitOfWorkMock.Verify(uow => uow.Repository<Payment>().SoftDeleteAsync(It.IsAny<Expression<Func<Payment, bool>>>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Never);
    }

    [Fact(DisplayName = "Test 05")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task GetAllPaymentAsync_ShouldReturnAllPayments()
    {
        // Arrange
        var payments = new List<Payment>
        {
            new Payment { Id = Guid.NewGuid(), Amount = 100, IsDeleted = false },
            new Payment { Id = Guid.NewGuid(), Amount = 200, IsDeleted = false }
        };

        var paymentDtos = payments.Select(p => new PaymentDto { Id = p.Id, Amount = p.Amount }).ToList();

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().GetAllAsync()).ReturnsAsync(payments);
        _mapperMock.Setup(x => x.Map<IEnumerable<PaymentDto>>(It.IsAny<IEnumerable<Payment>>())).Returns(paymentDtos);

        // Act
        var result = await _paymentService.GetAllPaymentAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(paymentDtos.Count, result.Data.Count());

    }

    [Fact(DisplayName = "Test 06")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task GetPaymentAsync_ShouldReturnPayment()
    {
        // Arrange
        var transactionId = "PAY-20250208224655442-f8a439d460ec4609ae77757e7f1fc22e";
        var payment = new Payment { Id = Guid.NewGuid(), TransactionId = transactionId, Amount = 100 };
        var paymentDto = new PaymentDto { Id = payment.Id, TransactionId = payment.TransactionId, Amount = payment.Amount };

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().FindAsync(It.IsAny<Expression<Func<Payment, bool>>>())).ReturnsAsync(payment);
        _mapperMock.Setup(x => x.Map<PaymentDto>(It.IsAny<Payment>())).Returns(paymentDto);

        // Act
        var result = await _paymentService.GetPaymentAsync(transactionId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(paymentDto.TransactionId, result.Data.TransactionId);
    }


    [Fact(DisplayName = "Test 07")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task CreatePaymentAsync_ShouldLogError_BadRequest()
    {
        // Arrange
        var paymentDto = new PaymentCreatDto { OrderId = Guid.NewGuid() };
        var order = new OrderDto { Id = paymentDto.OrderId, TotalAmount = 100 };

        var orderResult = await Result<OrderDto>.SuccessAsync(order);

        var apiResponse = new ApiResponse<Result<OrderDto>>(
            new HttpResponseMessage(HttpStatusCode.BadRequest),
            orderResult,
            new RefitSettings()
        );

        _oderApiMock.Setup(x => x.GetOrderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(apiResponse);

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().AddAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);

        _queueServiceMock.Setup(x => x.PaymentCreatedMessage).Returns(new Uri("queue:PaymentCreated"));

        // Act
        var result = await _paymentService.CreatePaymentAsync(paymentDto);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact(DisplayName = "Test 08")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task Teste08()
    {
        // Arrange
        var paymentDto = new PaymentCreatDto
        {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CreditCard = new CreditCardDto
            {
                Holder = "John Doe",
                CardNumber = "1234567890123456",
                ExpirationDate = "12/2022",
                SecurityCode = "123"
            }
        };

        var order = new OrderDto
        {
            Id = paymentDto.OrderId,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 100,
            PaymentToken = PaymentTokenService.GenerateToken()
        };

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = order.TotalAmount,
            Status = (int)PaymentStatus.Pending,
            Method = (int)PaymentMethod.CreditCard,
            PaymentDate = DateTime.Now,
            TransactionId = PaymentHelper.GenerateTransactionIdWithPrefix()
        };

        var orderResult = await Result<OrderDto>.SuccessAsync(order);

        var apiResponse = new ApiResponse<Result<OrderDto>>(
            new HttpResponseMessage(HttpStatusCode.OK),
            orderResult,
            new RefitSettings()
        );

        _oderApiMock.Setup(x => x.GetOrderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(apiResponse);

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().AddAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Repository<Payment>().FindAsync(It.IsAny<Expression<Func<Payment, bool>>>())).ReturnsAsync(payment);

        _queueServiceMock.Setup(x => x.PaymentCreatedMessage).Returns(new Uri("queue:PaymentCreated"));

        // Act
        var result = await _paymentService.CreatePaymentAsync(paymentDto);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "Test 09")]
    [Trait("Billing", "PaymentServiceTest")]
    public async Task Teste09()
    {
        // Arrange
        var paymentDto = new PaymentCreatDto
        {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CreditCard = new CreditCardDto
            {
                Holder = "John Doe",
                CardNumber = "1234567890123456",
                ExpirationDate = "12/2022",
                SecurityCode = "123"
            }
        };

        var order = new OrderDto
        {
            Id = paymentDto.OrderId,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 100,
            PaymentToken = PaymentTokenService.GenerateToken()
        };

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = order.TotalAmount,
            Status = (int)PaymentStatus.Completed,
            Method = (int)PaymentMethod.CreditCard,
            PaymentDate = DateTime.Now,
            TransactionId = PaymentHelper.GenerateTransactionIdWithPrefix()
        };

        var orderResult = await Result<OrderDto>.SuccessAsync(order);

        var apiResponse = new ApiResponse<Result<OrderDto>>(
            new HttpResponseMessage(HttpStatusCode.OK),
            orderResult,
            new RefitSettings()
        );

        _oderApiMock.Setup(x => x.GetOrderAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(apiResponse);

        _unitOfWorkMock.Setup(x => x.Repository<Payment>().AddAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Repository<Payment>().FindAsync(It.IsAny<Expression<Func<Payment, bool>>>())).ReturnsAsync(payment);

        _queueServiceMock.Setup(x => x.PaymentCreatedMessage).Returns(new Uri("queue:PaymentCreated"));

        // Act
        var result = await _paymentService.CreatePaymentAsync(paymentDto);

        // Assert
        Assert.True(result.Succeeded);
    }
}

