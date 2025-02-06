using Microsoft.Extensions.Configuration;

namespace Message.Broker.RabbitMq.Configurations;

public static class QueueConfig
{
    public static string PaymentCreatedMessage => $"casoft.payment.created.v1";

    public static string PaymentConfirmedMessage => $"casoft.payment.confirmed.v1";

    public static string PaymentFailedMessage => $"casoft.payment.failed.v1";

    public static string OrderCreatedMessage => $"casoft.order.created.v1";

    public static string ProductCreatedMessage => $"casoft.product.created.v1";

    public static string ProductUpdatedMessage => $"casoft.product.updated.v1";

    public static string ProductDeletedMessage => $"casoft.product.deleted.v1";

    public static string OrderDeletedMessage => $"casoft.order.deleted.v1";

    private static IConfigurationBuilder GetConfigBuilder()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddEnvironmentVariables();

        return builder;
    }

    public static string EnvironmentPrefix()
    {
        var configuration = GetConfigBuilder().Build();

        return configuration?.GetSection("RabbitMqTransportOptions:Prefix").Value!;
    }

}

