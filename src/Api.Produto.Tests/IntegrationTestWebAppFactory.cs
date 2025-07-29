using Infrastructure;
using MassTransit;
using Message.Broker.Abstractions;
using Message.Broker.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Product.Infrastructure.Consumers;
using Shared.Kernel.FluentMigrator;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Product.Api.Tests;
public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithUsername("admin")
        .WithPassword("Teste@123")
        .WithDatabase("Products")
        .Build();


    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithUsername("guest")
        .WithPassword("guest")
        .WithPortBinding(5672, 5672) // Porta padrão do RabbitMQ
        .WithImage("rabbitmq:3.12-management")
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        SetRabbitMqEnvironmentVariables();

        builder.ConfigureTestServices(services =>
        {
            ConfigurePostegreSqlFromTestcontainer(services);
            RemoveOriginalRabbitMqConfiguration(services);
            AddRabbitMqFromTestcontainer(services);
        });
    }
    private void ConfigurePostegreSqlFromTestcontainer(IServiceCollection services)
    {

        var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ProductDbContext));
        if (context != null)
        {
            services.Remove(context);
            var options = services.Where(r => (r.ServiceType == typeof(DbContextOptions))
              || (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))).ToArray();
            foreach (var option in options)
            {
                services.Remove(option);
            }
        }

        services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
        });

        services.AddFluentMigrationConfig(_postgreSqlContainer.GetConnectionString(),
        typeof(Infrastructure.Migrations.Inicio).Assembly);

    }

    private void RemoveOriginalRabbitMqConfiguration(IServiceCollection services)
    {
        // Remover configuração antiga
        var descriptorsToRemove = services
            .Where(s => s.ServiceType == typeof(IRabbitMqService) ||
                        s.ServiceType == typeof(IHostedService) && s.ImplementationType == typeof(RabbitMqHostedService))
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
            services.Remove(descriptor);

        // remove também as configurações prévias do MassTransit
        var massTransitDescriptors = services.Where(s =>
            s.ServiceType.FullName?.StartsWith("MassTransit") == true
        ).ToList();

        foreach (var descriptor in massTransitDescriptors)
            services.Remove(descriptor);
    }

    private void AddRabbitMqFromTestcontainer(IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqService, RabbitMqService>();
        services.AddHostedService<RabbitMqHostedService>();

        services.AddMassTransit(x =>
        {
            // Registra os consumers se necessário
            x.AddConsumers(typeof(ProductAddedConsumer).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(_rabbitMqContainer.Hostname, _rabbitMqContainer.GetMappedPublicPort(5672), "/", host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });

                // Aqui você configura as filas manualmente se necessário
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private void SetRabbitMqEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:Host", _rabbitMqContainer.Hostname);
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:Port", _rabbitMqContainer.GetMappedPublicPort(5672).ToString());
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:User", "guest");
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:Pass", "guest");
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:VHost", "/");
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:UseSsl", "false");
        Environment.SetEnvironmentVariable("RabbitMqTransportOptions:Prefix", "dev");
    }

    //alteration to use ConfigureAppConfiguration instead of SetRabbitMqEnvironmentVariables
    private void SetRabbitMqConfiguration(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var overrideConfig = new Dictionary<string, string?>
            {
                ["RabbitMqTransportOptions:Host"] = _rabbitMqContainer.Hostname,
                ["RabbitMqTransportOptions:Port"] = _rabbitMqContainer.GetMappedPublicPort(5672).ToString(),
                ["RabbitMqTransportOptions:User"] = "guest",
                ["RabbitMqTransportOptions:VHost"] = "/",
                ["RabbitMqTransportOptions:Pass"] = "guest",
                ["RabbitMqTransportOptions:UseSsl"] = "false",
                ["RabbitMqTransportOptions:Prefix"] = "dev"
            };

            configBuilder.AddInMemoryCollection(overrideConfig);
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync().ConfigureAwait(false);
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

}