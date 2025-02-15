using Auth.Api.Dtos.Login;
using ResultNet;
using Shared.Kernel.Models;

namespace Auth.Api.Abstractions;

public interface IAuthService
{
    Task<Result<LoginResponse>> GetToken(string email, string password);
    Task<UserInfoResponse> GetUserInfo(string accessToken);
    Task<Result<bool>> Logout(string refreshToken);
}
