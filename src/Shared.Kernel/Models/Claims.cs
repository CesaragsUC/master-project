using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.Models;

[ExcludeFromCodeCoverage]
public record Claims
{
    public string? Value { get; set; }
    public string? Type { get; set; }
}