using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Dtos.Login;

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
