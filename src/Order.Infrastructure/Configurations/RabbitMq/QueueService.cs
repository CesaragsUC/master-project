using Message.Broker.RabbitMq;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.RabbitMq;

[ExcludeFromCodeCoverage]
public class QueueService : IQueueService
{
    private readonly RabbitMqConfig _rabbitMqOptions;
    public QueueService(IOptions<RabbitMqConfig> options)
    {
        _rabbitMqOptions = new RabbitMqConfig();
        _rabbitMqOptions = options.Value;
    }

    public Uri OrderCreatedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.order.event.v1");
    public Uri OrderDeletedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.orderdeleted.event.v1");
    public Uri DeleteCartMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.removecart.event.v1");
}
