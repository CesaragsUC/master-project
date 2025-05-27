using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Models;

[ExcludeFromCodeCoverage]
public class OpenTelemetryOptions
{
    public string? Environment { get; set; }
    public string? AppName { get; set; }
    public Jaeger? Jaeger { get; set; }
    public OtlExporter? OtlExporter { get; set; }

    public GrafanaLoki? GrafanaLoki { get; set; }

}

[ExcludeFromCodeCoverage]
public class Jaeger
{
    public string? AgentHost { get; set; }
    public int AgentPort { get; set; }
}

[ExcludeFromCodeCoverage]
public class OtlExporter
{
    public string? EndPoint { get; set; }
    public string? Headers { get; set; }
}

[ExcludeFromCodeCoverage]
public class GrafanaLoki
{
    public string? EndPoint { get; set; }
}
