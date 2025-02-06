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

    public Uri OrderCreatedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.order.created.v1");
    public Uri OrderDeletedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.order.deleted.v1");
}
