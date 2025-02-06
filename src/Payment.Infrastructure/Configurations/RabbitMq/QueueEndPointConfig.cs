using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.Configurations.RabbitMq;

[ExcludeFromCodeCoverage]
public static class QueueEndPointConfig
{
    public static string PaymentCreatedMessage => $".casoft.payment.created.v1";

    public static string PaymentConfirmedMessage => $".casoft.payment.confirmed.v1";

    public static string PaymentFailedMessage => $".casoft.payment.failed.v1";
}
