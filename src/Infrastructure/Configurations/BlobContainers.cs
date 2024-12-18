using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Configurations
{
    [ExcludeFromCodeCoverage]
    public class BlobContainers
    {
        public string? ConnectionStrings { get; set; }
        public string? ContainerName { get; set; }
    }
}
