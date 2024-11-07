using Configurations;
using Domain.Interfaces;
using Infrastructure.Repository;
using InfraStructure;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Produtos.Consumer.Jobs;
using Quartz;
using System.Reflection;

namespace Produtos.Consumer.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.QuartzJobServices(configuration);
        services.MassTransitServices(configuration);
        services.MongoDbService(configuration);
        services.AddMediatrService();
        return services;
    }

    public static IServiceCollection QuartzJobServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("quartz");

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
                s.UseSqlServer(connectionString!);
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

    public static IServiceCollection MassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
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

    public static IServiceCollection MongoDbService(this IServiceCollection services, IConfiguration configuration)
    {
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
}
