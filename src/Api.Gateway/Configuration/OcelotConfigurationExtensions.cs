﻿using Ocelot.Configuration.File;

namespace Api.Gateway.Configuration;

//https://stackoverflow.com/questions/63421563/environment-variable-in-ocelot-config-file
//Configura o Ocelot para pegar os Hosts de modo dinamico.
public static class FileConfigurationExtensions
{
    public static IServiceCollection ConfigureDownstreamHostAndPortsPlaceholders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.PostConfigure<FileConfiguration>(fileConfiguration =>
        {
            var globalHosts = configuration
                .GetSection("OcelotConfiguration:Hosts")
                .Get<GlobalHosts>();

            foreach (var route in fileConfiguration.Routes)
            {
                ConfigureRote(route, globalHosts);
            }
        });

        return services;
    }

    private static void ConfigureRote(FileRoute route, GlobalHosts globalHosts)
    {
        foreach (var hostAndPort in route.DownstreamHostAndPorts)
        {
            var host = hostAndPort.Host;

            if (host.StartsWith("{") && host.EndsWith("}"))
            {
                var placeHolder = host.TrimStart('{').TrimEnd('}');
                if (globalHosts.TryGetValue(placeHolder, out var uri))
                {
                    route.DownstreamScheme = uri.Scheme;
                    hostAndPort.Host = uri.Host;
                }
            }
        }
    }
}

public class GlobalHosts : Dictionary<string, Uri> { }