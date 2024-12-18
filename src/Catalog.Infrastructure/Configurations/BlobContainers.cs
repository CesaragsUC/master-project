using System.Diagnostics.CodeAnalysis;

namespace  Catalog.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public class BlobContainers
{
    public string? ConnectionStrings { get; set; }
    public string? ContainerName { get; set; }
}
