using Application.Dtos.Dtos.Payments;
using AutoMapper;
using Billing.Domain.Entities;
using Billing.Service.Abstractions;
using Billing.Service.Extentions;
using MassTransit;
using Message.Broker.RabbitMq;
using Message.Broker.RabbitMq.Configurations;
using Messaging.Contracts.Events.Payments;
using Microsoft.Extensions.Options;
using RepoPgNet;
using ResultNet;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Service;

[ExcludeFromCodeCoverage]
public class PaymentService : IPaymentService
{
    private readonly IMapper _mapper;
    private readonly IPgRepository<Order> _repository;
    private readonly RabbitMqConfig _rabbitMqOptions;
    private readonly IOderApi _oderApi;

    public PaymentService(IMapper mapper,
        IPgRepository<Order> repository,
        IOptions<RabbitMqConfig> options,
        IOderApi oderApi)
    {
        _mapper = mapper;
        _repository = repository;
        _rabbitMqOptions = options.Value;
        _oderApi = oderApi;
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

        var instance = RabbitMqSingleton.GetInstance(_rabbitMqOptions.Host);

        var messageEndpoint = await instance.Bus.GetSendEndpoint(new Uri($"queue:{_rabbitMqOptions.Prefix}{QueueConfig.PaymentCreatedMessage}"));
        await messageEndpoint.Send(paymentCreated);

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
