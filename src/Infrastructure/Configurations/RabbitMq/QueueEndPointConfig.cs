using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.Configurations.RabbitMq;

[ExcludeFromCodeCoverage]
public static class QueueEndPointConfig
{
    public static string ProductCreatedMessage => $".casoft.product.created.v1";

    public static string ProductUpdatedMessage => $".casoft.product.updated.v1";

    public static string ProductDeletedMessage => $".casoft.product.deleted.v1";
}
