using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Models;

[ExcludeFromCodeCoverage]
public record UserToken
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public IEnumerable<Claims>? Claims { get; set; }
    public IEnumerable<string>? Groups { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
