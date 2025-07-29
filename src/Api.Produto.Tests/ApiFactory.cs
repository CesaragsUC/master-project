using DotNet.Testcontainers.Builders;
using Infrastructure;
using Message.Broker.Abstractions;
using Message.Broker.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Product.Infrastructure.RabbitMq;
using Shared.Kernel.FluentMigrator;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Product.Api.Tests;
public class ApiFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    private IConfiguration? _configuration;

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithUsername("admin")
        .WithPassword("Teste@123")
        .WithDatabase("Products")
        .Build();


    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.12-management")
        .WithUsername("admin")
        .WithPassword("admin")
        .WithCommand("rabbitmq-server", "rabbitmq-plugins", "enable", "rabbitmq_management")
        .WithCleanUp(true)
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        SetTestsConfiguration(builder);

        builder.ConfigureTestServices(services =>
        {
            ConfigurePostegreSqlFromTestcontainer(services);
            ConfigureRabbitMqFromTestcontainer(services);
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


    private void ConfigureRabbitMqFromTestcontainer(IServiceCollection services)
    {
        //remove a configuração original do RabbitMqService
        RemoveOriginalRabbitMqConfiguration(services);

        //registra novamente o RabbitMqService
        services.AddSingleton<IRabbitMqService, RabbitMqService>();

        services.AddMessageBrokerSetup(_configuration);
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

    private void SetTestsConfiguration(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var port = _rabbitMqContainer.GetMappedPublicPort(5672);
            var hostname = _rabbitMqContainer.Hostname;
            var connectionString = _rabbitMqContainer.GetConnectionString();
            var overrideConfig = new Dictionary<string, string?>
            {
                ["RabbitMqTransportOptions:Host"] = connectionString,
                ["RabbitMqTransportOptions:Port"] = port.ToString(),
                ["RabbitMqTransportOptions:User"] = "admin",
                ["RabbitMqTransportOptions:VHost"] = "/",
                ["RabbitMqTransportOptions:Pass"] = "admin",
                ["RabbitMqTransportOptions:UseSsl"] = "false",
                ["RabbitMqTransportOptions:Prefix"] = "dev",
                ["BlobContainers:ConnectionStrings"] = "UseDevelopmentStorage=true",
                ["BlobContainers:ContainerName"] = "meu-container"
            };

            configBuilder.AddInMemoryCollection(overrideConfig);
            _configuration = configBuilder.Build();
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