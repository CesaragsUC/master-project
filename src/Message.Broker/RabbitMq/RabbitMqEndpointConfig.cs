using System.Diagnostics.CodeAnalysis;

namespace Message.Broker.RabbitMq;

/// <summary>
/// RabbitMq Endpoint Configuration
/// </summary>
[ExcludeFromCodeCoverage]
public class RabbitMqEndpointConfig
{
    public string? QueueName { get; set; }
    public string? RoutingKey { get; set; }
    public string? ExchangeType { get; set; }
    public int RetryLimit { get; set; }
    public TimeSpan Interval { get; set; }
    public bool ConfigureConsumeTopology { get; set; }
    public int PrefetchCount { get; set; }
    public int PrefetchLimit { get; set; }
}