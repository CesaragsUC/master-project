namespace Billing.Infrastructure.Configurations.RabbitMq;

public interface IQueueService
{
    Uri PaymentCreatedMessage { get; }
    Uri PaymentConfirmedMessage { get; }
    Uri PaymentFailedMessage { get; }
    Uri OrderUpdateMessage { get; }
}
