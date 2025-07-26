using Grafana.OpenTelemetry;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;
using Shared.Kernel.Models;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Opentelemetry;

[ExcludeFromCodeCoverage]
public static class OpenTelemetrySetup
{
    public static IServiceCollection OpenTelemetryConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var openTelemetryOptions = new OpenTelemetryOptions();
        configuration.GetSection("OpenTelemetryOptions").Bind(openTelemetryOptions);

        // configure metrics for grafana
        

        var otel = services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource =>
        {
            resource.AddService(serviceName: $"{openTelemetryOptions.AppName}");
            var globalOpenTelemetryAttributes = new List<KeyValuePair<string, object>>();
            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("env", openTelemetryOptions.Environment!));
            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("appName", openTelemetryOptions.AppName!));
            resource.AddAttributes(globalOpenTelemetryAttributes);
        });

        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
            .AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(openTelemetryOptions.OtlExporter!.EndPoint!);
                otlpOptions.Headers = !string.IsNullOrEmpty(openTelemetryOptions.OtlExporter.Headers) ?
                                            openTelemetryOptions?.OtlExporter?.Headers :
                                            string.Empty;
            })
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter(openTelemetryOptions.AppName!)
            .AddHttpClientInstrumentation()
            .AddNpgsqlInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddMeter(InstrumentationOptions.MeterName)
            .AddConsoleExporter()
            .AddPrometheusExporter());

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddEntityFrameworkCoreInstrumentation(opt => {
                opt.SetDbStatementForText = true; // Set the SQL statement for text queries
                opt.SetDbStatementForStoredProcedure = true; // Set the SQL statement for stored procedures
                opt.EnrichWithIDbCommand = (activity, command) =>
                {
                    // Enrich the activity with the command text
                    activity.SetTag("db.statement", command.CommandText);
                };
            });
            tracing.AddNpgsql();
            tracing.AddRedisInstrumentation();
            tracing.AddSource(openTelemetryOptions.AppName!);
            tracing.AddSource(DiagnosticHeaders.DefaultListenerName);//MassTransit
            tracing.AddConsoleExporter();
            tracing.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(openTelemetryOptions.OtlExporter!.EndPoint!);
                otlpOptions.Headers = !string.IsNullOrEmpty(openTelemetryOptions.OtlExporter.Headers) ?
                                                            openTelemetryOptions.OtlExporter.Headers :
                                                            string.Empty;
            });

        });

        return services;
    }

    public static void GrafanaOpenTelemetrySetup()
    {
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .UseGrafana()
                .Build();

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .UseGrafana()
            .AddProcessInstrumentation()
            .AddPrometheusHttpListener()
            .Build();
    }


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

    public static void SetupLoggingOtlp(WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Logging.AddOpenTelemetry( logging =>
        {
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
        });
    }
}
