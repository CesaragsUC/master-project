using Serilog.Core;
using Serilog.Events;

namespace Shared.Kernel.Models;

public class SerilogEnricher : ILogEventEnricher
{
    private readonly string _ApplicationName;
    private readonly string _EnvironmentName;

    public SerilogEnricher(string applicationName, string environmentName)
    {
        _ApplicationName = applicationName;
        _EnvironmentName = environmentName;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ApplicationName", _ApplicationName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Environment", _EnvironmentName));
    }
}