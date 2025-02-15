using System.Diagnostics.CodeAnalysis;

namespace Api.Gateway.Dtos;

[ExcludeFromCodeCoverage]
public class FrontEndUri
{
    public string? Uri { get; set; }
    public string? Name { get; set; }
}
