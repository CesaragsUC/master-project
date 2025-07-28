using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Product.Application.Handlers.Product;
using Product.Application.Services;
using Product.Domain.Abstractions;
using Product.Infrastructure.Repository;
using Shared.Kernel.FluentMigrator;
using Testcontainers.PostgreSql;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected ISender Sender;
    protected ProductDbContext DbContext;

    private ServiceProvider _serviceProvider;
    private PostgreSqlContainer _postgreSqlContainer;
    private IBobStorageService _bobStorageService;

    public async Task InitializeAsync()
    {
        // Starta o container
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername("admin")
            .WithPassword("Teste@123")
            .WithDatabase("Products")
            .WithPortBinding(0, true)
            .Build();

        await _postgreSqlContainer.StartAsync();


        // Monta a connection string dinâmica com base na porta exposta
        var connectionString = _postgreSqlContainer.GetConnectionString();

        // Configura os serviços
        var services = new ServiceCollection();

        // Remover o contexto existente
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ProductDbContext>));

        if (descriptor != null)
            services.Remove(descriptor);

        services.AddScoped<IBobStorageService, BobStorageService>();

        services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });


        // Cria a base no banco (caso precise rodar migrations aqui)
        services.AddFluentMigrationConfig(connectionString,
                typeof(Product.Infrastructure.Migrations.Inicio).Assembly);

        _serviceProvider = services.BuildServiceProvider();

        // Resolve os serviços
        var scope = _serviceProvider.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        Sender = scope.ServiceProvider.GetRequiredService<ISender>();


    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        if (_serviceProvider is IDisposable d)
        {
            d.Dispose();
        }
    }

    protected void InitializeMediatrService()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateProductHandler).Assembly))
            .BuildServiceProvider();

    }
}
