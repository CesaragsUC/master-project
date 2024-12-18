using System.Diagnostics.CodeAnalysis;

namespace Configurations;

[ExcludeFromCodeCoverage]
public class MongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}
