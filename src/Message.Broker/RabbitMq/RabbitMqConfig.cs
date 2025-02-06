using MassTransit;
using System.Diagnostics.CodeAnalysis;

namespace Message.Broker.RabbitMq;

/// <summary>
/// Is a class to extend the RabbitMqTransportOptions
/// Here we can add more properties to the RabbitMqTransportOptions
/// </summary>
[ExcludeFromCodeCoverage]
public class RabbitMqConfig : RabbitMqTransportOptions
{
    public string? Prefix { get; set; }
}
