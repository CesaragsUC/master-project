﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.Abstractions;
using Product.Application.Services;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Product.Application.Configurations;


//https://github.com/quartznet/quartznet/tree/main/database/tables
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IQueueService, QueueService>();
        services.AddMediatrService();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }


    public static void AddMediatrService(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
    }
}
