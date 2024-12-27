using Catalog.Domain.Abstractions;
using Catalog.Infrastructure.Configurations;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Repository;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Consumer.Jobs;
using Quartz;
using System.Reflection;

namespace Product.Consumer.Configurations;


//https://github.com/quartznet/quartznet/tree/main/database/tables

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.QuartzJobServices();
        services.MassTransitServices();
        services.MongoDbService();
        services.AddMediatrService();
        return services;
    }

    public static IServiceCollection QuartzJobServices(this IServiceCollection services)
    {
        var configuration = GetConfigBuilder().Build();

        var connectionString = configuration.GetConnectionString("PostgreSql");

        services.AddQuartz(q =>
        {
            // Configurações do Scheduler
            q.SchedulerName = "MassTransit-Scheduler";
            q.SchedulerId = "AUTO";

            //Quantidade máxima de tarefas que o Thread Pool do Quartz pode executar simultaneamente.
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });


            // Configuração de Persistência
            q.UsePersistentStore(s =>
            {
                s.UseProperties = true;
                s.RetryInterval = TimeSpan.FromSeconds(15);
                s.UsePostgres(connectionString!);
                s.UseNewtonsoftJsonSerializer();

                s.UseClustering(c =>
                {
                    c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    c.CheckinInterval = TimeSpan.FromSeconds(10);
                });
            });

            // Configura o job ProdutoJob
            var jobKey = new JobKey("ProdutoJob");
            q.AddJob<ProdutoJob>(opts => opts.WithIdentity(jobKey));

            // Configura o trigger para o job
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ProdutoJob")
                .StartNow() // O job começará imediatamente quando o scheduler for iniciado.
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));// A cada 10 segundos o job será executado.
        });

        services.AddQuartzHostedService(options =>
        {
            options.StartDelay = TimeSpan.FromSeconds(5);
            options.WaitForJobsToComplete = true;
        });

        services.AddScoped<ProdutoJob>();

        return services;
    }

    public static IServiceCollection MassTransitServices(this IServiceCollection services)
    { 

        var configuration = GetConfigBuilder().Build();

        var myConnString = configuration.GetSection("RabbitMqTransport");

        services.Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMqTransport"));

        services.AddMassTransit(x =>
        {
            x.AddPublishMessageScheduler();

            x.AddQuartzConsumers();

            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UsePublishMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        });


        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
        });

        return services;
    }

    public static IServiceCollection MongoDbService(this IServiceCollection services)
    {
        var configuration = GetConfigBuilder().Build();

        var myConnString = configuration.GetSection(nameof(MongoDbSettings));

        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddSingleton<IMongoDbContext,MongoDbContext>();

        return services;
    }

    public static void AddMediatrService(this IServiceCollection services)
    {
        //Registra todos os handlers do MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
    }

    private static IConfigurationBuilder GetConfigBuilder()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddEnvironmentVariables();

        return builder;
    }
}
