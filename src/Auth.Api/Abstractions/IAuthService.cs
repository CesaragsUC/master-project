using Application.Dtos.Dtos.Login;

namespace Auth.Api.Abstractions;

public interface IAuthService
{
    Task<TokenResponse> GetToken(string email, string password);
    Task<bool> Logout(string refreshToken);
}
