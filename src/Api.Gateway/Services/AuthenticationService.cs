namespace Api.Gateway.Services;

using Api.Gateway.Dto;
using Api.Gateway.Dtos;
using Microsoft.Extensions.Options;
using Serilog;
using Shared.Kernel.Models;
using System.Net.Http;
using System.Net.Http.Json; // para usar JsonContent
using System.Text.Json;

public class AuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<EndPointUri> authUri;

    public AuthenticationService(HttpClient httpClient,
        IOptions<EndPointUri> endPoint)
    {
        _httpClient = httpClient;
        authUri = endPoint;
    }

    public async Task<TokenResponse> Authenticate(LoginDto loginDto)
    {
        try
        {
            var requestContent = JsonContent.Create(loginDto);
            Log.Information( "Request to Uri:", authUri.Value.AuthApi);

            var response = await _httpClient.PostAsync($"{authUri.Value.AuthApi}/login", requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TokenResponse>(responseContent);
            }
            else
            {
                return null!;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occour during request authentication", ex.Message);
            throw;
        }

    }

    public async Task<string> Logout(string refreshToken)
    {
        try
        {
            var requestContent = JsonContent.Create(refreshToken);

            var response = await _httpClient.PostAsync($"{authUri.Value.AuthApi}/logout", requestContent);

            if (response.IsSuccessStatusCode)
            {
               return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "Fail to request logout.";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occour during request logout", ex.Message);
            throw;
        }


    }
}
