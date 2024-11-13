namespace Api.Gateway.Services;

using Api.Gateway.Dto;
using Serilog;
using System.Net.Http;
using System.Net.Http.Json; // para usar JsonContent

public class AuthenticationService
{
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Authenticate(LoginDto loginDto)
    {
        try
        {
            var uri = "https://localhost:7123/api/auth/login";

            var requestContent = JsonContent.Create(loginDto);

            var response = await _httpClient.PostAsync(uri, requestContent);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Log.Error("An error occour during request authentication",ex);
            throw;
        }

    }

    public async Task Logout(string refreshToken)
    {
        try
        {
            var uri = "https://localhost:7123/api/auth/logout";

            var requestContent = JsonContent.Create(refreshToken);

            var response = await _httpClient.PostAsync(uri, requestContent);
        }
        catch (Exception ex)
        {
            Log.Error("An error occour during request logout", ex);
            throw;
        }


    }
}
