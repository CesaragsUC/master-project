using DotNet.Testcontainers.Builders;
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
using Product.Infrastructure.RabbitMq;
using Shared.Kernel.FluentMigrator;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Product.Api.Tests;
public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    private IConfiguration _configuration;

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithUsername("admin")
        .WithPassword("Teste@123")
        .WithDatabase("Products")
        .Build();


    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithUsername("guest")
        .WithPassword("guest")
        .WithImage("rabbitmq:3.12-management")
        .WithPortBinding(5672)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
        .WithCleanUp(true)
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        SetRabbitMqConfiguration(builder);

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

        services.AddMessageBrokerSetup(_configuration);
    }

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
            _configuration = configBuilder.Build();
        });

    }

    public async Task InitializeAsync()
    {
        try
        {
            // Check if we're in GitHub Actions
            var isGitHubActions = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

            if (isGitHubActions)
            {
                Console.WriteLine("Running in GitHub Actions environment - setting up containers with special configuration");
                // Additional logging to help with troubleshooting in GitHub Actions
            }

            await _postgreSqlContainer.StartAsync();
            await _rabbitMqContainer.StartAsync();

            // Log important container information for debugging
            Console.WriteLine($"RabbitMQ Container Status: Running at {_rabbitMqContainer.Hostname}:{_rabbitMqContainer.GetMappedPublicPort(5672)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize containers: {ex.Message}");
            throw;
        }
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

}