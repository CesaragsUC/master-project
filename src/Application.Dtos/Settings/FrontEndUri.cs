using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Settings;

[ExcludeFromCodeCoverage]
public class FrontEndUri
{
    public string? Uri { get; set; }
    public string? Name { get; set; }
}
