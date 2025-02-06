using MassTransit;

namespace Message.Broker.Extensions;

public static class RabbitMqTransportOptionsExtension
{
    public static string Prefix(this RabbitMqTransportOptions transportOptions)
    {
        return string.Empty;
    }
}
