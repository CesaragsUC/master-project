using Message.Broker.RabbitMq;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
namespace Basket.Infrastructure.RabbitMq;

[ExcludeFromCodeCoverage]
public class QueueService : IQueueService
{
    private readonly RabbitMqConfig _rabbitMqOptions;
    public QueueService(IOptions<RabbitMqConfig> options)
    {
        _rabbitMqOptions = new RabbitMqConfig();
        _rabbitMqOptions = options.Value;
    }

    public Uri OrderCheckoutEventMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.ordercheckout.event.v1");
    public Uri OrderDeletedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.order.deleted.v1");
}
