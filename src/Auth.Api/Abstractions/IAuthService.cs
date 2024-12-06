using Application.Dtos.Dtos.Login;
using Application.Dtos.Dtos.Response;

namespace Auth.Api.Abstractions;

public interface IAuthService
{
    Task<Result<LoginResponse>> GetToken(string email, string password);
    Task<UserInfoResponse> GetUserInfo(string accessToken);
    Task<Result<bool>> Logout(string refreshToken);
}
