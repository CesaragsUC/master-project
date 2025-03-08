using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Shared.Kernel.Models;

[ExcludeFromCodeCoverage]
public record LoginResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsAdmin { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public double ExpiresIn { get; set; }
    public UserToken? UserToken { get; set; }
}
