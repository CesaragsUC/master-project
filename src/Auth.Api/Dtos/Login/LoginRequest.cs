using System.Diagnostics.CodeAnalysis;

namespace Auth.Api.Dtos.Login;

[ExcludeFromCodeCoverage]
public record LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

