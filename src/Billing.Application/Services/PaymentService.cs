using Application.Dtos.Dtos.Payments;
using AutoMapper;
using Billing.Application.Abstractions;
using Billing.Application.Extentions;
using Billing.Domain.Entities;
using Billing.Infrastructure.Configurations.RabbitMq;
using Message.Broker.Abstractions;
using Messaging.Contracts.Events.Payments;
using RepoPgNet;
using ResultNet;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Service;

[ExcludeFromCodeCoverage]
public class PaymentService : IPaymentService
{
    private readonly IMapper _mapper;
    private readonly IPgRepository<Order> _repository;
    private readonly IQueueService _queueService;
    private readonly IOderApi _oderApi;
    private readonly IRabbitMqService _rabbitMqService;

    public PaymentService(IMapper mapper,
        IPgRepository<Order> repository,
       IQueueService queueService,
        IOderApi oderApi,
        IRabbitMqService rabbitMqService)
    {
        _mapper = mapper;
        _repository = repository;
        _queueService = queueService;
        _oderApi = oderApi;
        _rabbitMqService = rabbitMqService;
    }


    public async Task<Result<bool>> CreatePaymentAsync(PaymentCreatDto paymentDto)
    {
        var payment = paymentDto.ToPayment();

        var orderResponse = await _oderApi.GetOrderAsync(paymentDto.OrderId);

        if (!orderResponse.IsSuccessStatusCode)
        {
            return await Result<bool>.FailureAsync("Order not founded");
        }

        payment.Amount = orderResponse!.Content!.Data.TotalAmount;
        payment.Status = (int)PaymentStatus.Pending;
        payment.Method = (int)PaymentMethod.CreditCard;
        payment.PaymentDate = DateTime.Now;

        var paymentCreated = new PaymentCreatedEvent
        {

            OrderId = orderResponse.Content.Data.Id,
            PaymentToken = PaymentTokenService.GenerateToken(),
            Amount = orderResponse!.Content!.Data.TotalAmount,
            CustomerId = orderResponse!.Content!.Data.CustomerId,
            PaymentDate = DateTime.Now,
            Status = (int)PaymentStatus.Pending,
            PaymentMethod = (int)PaymentMethod.CreditCard,
            CreditCard = new Messaging.Contracts.Events.Payments.CreditCard
            {
                CardNumber = payment!.CreditCard!.CardNumber,
                ExpirationDate = payment.CreditCard.ExpirationDate,
                SecurityCode = payment.CreditCard.SecurityCode,
                Holder = payment.CreditCard.Holder,
            }
        };

        await _rabbitMqService.Send(paymentCreated, _queueService.PaymentCreatedMessage);

        return await Result<bool>.SuccessAsync("Payment created");
    }

    public async Task<Result<bool>> DeletePaymentAsync(string transactionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<PaymentDto>>> GetAllPaymentAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PaymentDto>> GetPaymentAsync(string transactionId)
    {
        throw new NotImplementedException();
    }
}
