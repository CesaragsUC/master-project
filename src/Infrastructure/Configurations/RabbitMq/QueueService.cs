using Message.Broker.RabbitMq;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.RabbitMq;

[ExcludeFromCodeCoverage]
public class QueueService : IQueueService
{
    private readonly RabbitMqConfig _rabbitMqOptions;
    public QueueService(IOptions<RabbitMqConfig> options)
    {
        _rabbitMqOptions = new RabbitMqConfig();
        _rabbitMqOptions = options.Value;
    }

    public Uri ProductCreatedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.productadded.event.v1");

    public Uri ProductUpdatedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.produtctupdated.event.v1");

    public Uri ProductDeletedMessage => new Uri($"queue:{_rabbitMqOptions.Prefix}.casoft.produtctdeleted.event.v1");
}
