//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using OpenTelemetry.Metrics;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;
//using Shared.Kernel.Models;
//using System.Diagnostics.CodeAnalysis;

//namespace Product.Infrastructure.Configurations.Grafana;

////https://medium.com/@jwag/grafana-with-logging-and-metrics-from-a-dotnet-api-cbaa06d359aa
////https://github.com/sgbj/dotnet-monitoring
////https://github.com/jaaywags/grafana-dotnet-demo
////https://github.com/naeemaei/MonitoringExample/tree/master

//[ExcludeFromCodeCoverage]
//public static class GrafanaConfig
//{
//    public static IServiceCollection AddGrafanaSetup(this IServiceCollection services,
//    IConfiguration configuration)
//    {
//        var openTelemetryOptions = new OpenTelemetryOptions();
//        configuration.GetSection("OpenTelemetryOptions").Bind(openTelemetryOptions);

//        // configure metrics for grafana
//        var otel = services.AddOpenTelemetry();

//        // Configure OpenTelemetry Resources with the application name
//        otel.ConfigureResource(resource =>
//        {
//            resource.AddService(serviceName: $"{openTelemetryOptions.AppName}");
//            var globalOpenTelemetryAttributes = new List<KeyValuePair<string, object>>();
//            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("env", openTelemetryOptions.Environment!));
//            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("appName", openTelemetryOptions.AppName!));
//            resource.AddAttributes(globalOpenTelemetryAttributes);
//        });

//        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
//        otel.WithMetrics(metrics => metrics
//            .AddOtlpExporter(otlpOptions =>
//            {
//                otlpOptions.Endpoint = new Uri(openTelemetryOptions.OtlExporter!.EndPoint!);
//            })
//            // Metrics provider from OpenTelemetry
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddRuntimeInstrumentation()
//            .AddProcessInstrumentation()
//            .AddMeter(openTelemetryOptions.Environment!)
//            // Metrics provides by ASP.NET Core in .NET 8
//            .AddMeter("Microsoft.AspNetCore.Hosting")
//            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
//            .AddPrometheusExporter());

//        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
//        otel.WithTracing(tracing =>
//        {
//            tracing.AddAspNetCoreInstrumentation();
//            tracing.AddHttpClientInstrumentation();
//            tracing.AddSource(openTelemetryOptions.Environment!);
//            tracing.AddOtlpExporter(otlpOptions =>
//            {
//                otlpOptions.Endpoint = new Uri(openTelemetryOptions.OtlExporter!.EndPoint!);
//            });
//        });

//        return services;
//    }
//}
