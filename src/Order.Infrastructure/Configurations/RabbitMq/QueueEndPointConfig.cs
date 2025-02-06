using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.Configurations.RabbitMq;

[ExcludeFromCodeCoverage]
public static class QueueEndPointConfig
{
    public static string OrderCreatedMessage => $".casoft.order.created.v1";
    public static string OrderDeletedMessage => $".casoft.order.deleted.v1";
}
