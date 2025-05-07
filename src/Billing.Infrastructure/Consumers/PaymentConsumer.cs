using AutoMapper;
using Billing.Domain.Abstractions;
using Billing.Domain.Execeptions;
using MassTransit;
using Message.Broker.Abstractions;
using Message.Broker.RabbitMq;
using Messaging.Contracts.Events.Payments;
using Microsoft.Extensions.Options;
using Serilog;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.Consumers;

[ExcludeFromCodeCoverage]
public class PaymentConsumer : IConsumer<PaymentCreatedEvent>
{
    private readonly IMapper _mapper;
    private readonly IQueueService _queueService;
    private readonly IRabbitMqService _rabbitMqService;

    public PaymentConsumer(IMapper mapper,
        IOptions<RabbitMqConfig> options,
        IQueueService queueService,
        IRabbitMqService rabbitMqService)
    {
        _mapper = mapper;
        _queueService = queueService;
        _rabbitMqService = rabbitMqService;
    }

    public async Task Consume(ConsumeContext<PaymentCreatedEvent> context)
    {
        try
        {
            if (!PaymentTokenService.ValidateToken(context?.Message?.PaymentToken!))
            {
                throw new InvalidPaymentTokenException("Invalid payment token");
            }

            bool paymentResult = Random.Shared.Next(0, 2) == 1;

            if (paymentResult)
            {
                var paymentSucceedMessage = new PaymentConfirmedEvent()
                {
                    PaymentToken = context!.Message.PaymentToken,
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.Amount,
                    PaymentDate = context.Message.PaymentDate,
                    PaymentMethod = context.Message.PaymentMethod,
                    Status = (int)PaymentStatus.Completed
                };

                await _rabbitMqService.Send(paymentSucceedMessage, _queueService.PaymentConfirmedMessage);
            }
            else
            {

                var paymentFailedMessage = new PaymentFailedEvent()
                {
                    PaymentToken = context!.Message.PaymentToken,
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.Amount,
                    PaymentDate = context.Message.PaymentDate,
                    PaymentMethod = context.Message.PaymentMethod,
                    Status = (int)PaymentStatus.Failed
                };

                await _rabbitMqService.Send(paymentFailedMessage, _queueService.PaymentFailedMessage);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while send payment message");
        }

        await Task.CompletedTask;
    }
}
