using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public static IServiceCollection AddGrafanaSetup(this IServiceCollection services,
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
            })
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            .AddMeter(openTelemetryOptions.AppName!)
            .AddRuntimeInstrumentation()
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddPrometheusExporter());

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource(openTelemetryOptions.AppName!);
            tracing.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(openTelemetryOptions.OtlExporter!.EndPoint!);
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
}
