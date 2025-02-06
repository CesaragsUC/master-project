using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Dtos.Login;

[ExcludeFromCodeCoverage]
public record Claims
{
    public string? Value { get; set; }
    public string? Type { get; set; }
}