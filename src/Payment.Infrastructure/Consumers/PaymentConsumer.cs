using AutoMapper;
using MassTransit;
using Message.Broker.RabbitMq;
using Message.Broker.RabbitMq.Configurations;
using Messaging.Contracts.Events.Payments;
using Microsoft.Extensions.Options;
using Serilog;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;

namespace Billing.Infrastructure.Consumers;

public class PaymentConsumer : IConsumer<PaymentCreatedEvent>
{
    private readonly IMapper _mapper;
    private readonly RabbitMqConfig _rabbitMqOptions;

    public PaymentConsumer(IMapper mapper,
        IOptions<RabbitMqConfig> options)
    {
        _mapper = mapper;
        _rabbitMqOptions = options.Value;
        _rabbitMqOptions.Prefix = "dev.";//arrumar isso depoiis esta vindo null
    }

    public async Task Consume(ConsumeContext<PaymentCreatedEvent> context)
    {
        try
        {
            if (!PaymentTokenService.ValidateToken(context?.Message?.PaymentToken!))
            {
                throw new InvalidPaymentTokenException("Invalid payment token");
            }

            // Simular processamento de pagamento
            bool paymentSuccess = new Random().Next(0, 2) == 1;

            if (paymentSuccess)
            {
                // Enviar para fila de pagamento autorizado payment.success.queue
                var instance = RabbitMqSingleton.GetInstance(_rabbitMqOptions.Host);

                var paymentSucceedMessage = new PaymentConfirmedEvent()
                {
                    PaymentToken = context.Message.PaymentToken,
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.Amount,
                    PaymentDate = context.Message.PaymentDate,
                    PaymentMethod = context.Message.PaymentMethod,
                    Status = (int)PaymentStatus.Complete
                };

                var messageEndpoint = await instance.Bus.GetSendEndpoint(new Uri($"queue:{_rabbitMqOptions.Prefix}{QueueConfig.PaymentConfirmedMessage}"));
                await messageEndpoint.Send(paymentSucceedMessage);
            }
            else
            {
                // Enviar para fila de pagamento falho payment.failed.queue
                var instance = RabbitMqSingleton.GetInstance(_rabbitMqOptions.Host);

                var paymentFailedMessage = new PaymentFailedEvent()
                {
                    PaymentToken = context.Message.PaymentToken,
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.Amount,
                    PaymentDate = context.Message.PaymentDate,
                    PaymentMethod = context.Message.PaymentMethod,
                    Status = (int)PaymentStatus.Failed
                };

                var messageEndpoint = await instance.Bus.GetSendEndpoint(new Uri($"queue:{_rabbitMqOptions.Prefix}{QueueConfig.PaymentFailedMessage}"));
                await messageEndpoint.Send(paymentFailedMessage);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while send payment message");
        }

        await Task.CompletedTask;
    }
}

public class InvalidPaymentTokenException : Exception
{
    public InvalidPaymentTokenException(string message) : base(message)
    {
    }
}
