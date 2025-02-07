using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Dtos.Login;

[ExcludeFromCodeCoverage]
public record LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

