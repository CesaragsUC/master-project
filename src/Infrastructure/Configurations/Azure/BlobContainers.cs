using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.Configurations.Azure
{
    [ExcludeFromCodeCoverage]
    public class BlobContainers
    {
        public string? ConnectionStrings { get; set; }
        public string? ContainerName { get; set; }
    }
}
