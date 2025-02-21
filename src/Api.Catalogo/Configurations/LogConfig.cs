using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;
using Shared.Kernel.Models;
using System.Diagnostics.CodeAnalysis;

namespace Catalogo.Api.Configurations;

[ExcludeFromCodeCoverage]
public static class LogConfig
{
    public static void SetupLogging(WebApplicationBuilder builder, IConfiguration configuration)
    {
        var openTelemetryOptions = new OpenTelemetryOptions();
        configuration.GetSection("OpenTelemetryOptions").Bind(openTelemetryOptions);

        builder.Host.UseSerilog((context, config) =>
        {
            config.Enrich.FromLogContext();
            config.Enrich.WithExceptionDetails();
            config.Enrich.With(new SerilogEnricher(
              openTelemetryOptions.AppName!,
              openTelemetryOptions.Environment!
            ));

            config
              .WriteTo.GrafanaLoki
              (
                openTelemetryOptions?.GrafanaLoki?.EndPoint!,
                new List<LokiLabel> {
                new() { Key = "appName", Value = openTelemetryOptions.AppName! },
                new() { Key = "env", Value = openTelemetryOptions.Environment! }
                }
              )
              .WriteTo.Console
              (
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} level=[{Level:u3}] appId={ApplicationId} appName={ApplicationName} env={Environment} {Message:lj} {NewLine}{Exception}"
              );
            config.ReadFrom.Configuration(context.Configuration);
        });
    }

}
