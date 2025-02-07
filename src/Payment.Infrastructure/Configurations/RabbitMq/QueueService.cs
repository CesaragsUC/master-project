using Message.Broker.RabbitMq;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.Configurations.RabbitMq;

[ExcludeFromCodeCoverage]
public class QueueService : IQueueService
{
    private readonly RabbitMqConfig _rabbitMqOptions;
    public QueueService(IOptions<RabbitMqConfig> options)
    {
        _rabbitMqOptions = new RabbitMqConfig();
        _rabbitMqOptions = options.Value;
    }

    public Uri PaymentCreatedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.payment.event.v1");
    public Uri PaymentConfirmedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.paymentconfirmed.event.v1");
    public Uri PaymentFailedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.paymentfailed.event.v1");
}
