using Auth.Api.Abstractions;
using Auth.Api.Dtos.Login;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthKeyCloakService _authService;

    public AuthController(ILogger<AuthController> logger,
        IAuthKeyCloakService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var tokenResponse = await _authService.GetToken(loginRequest.Email!, loginRequest.Password!);

        if (!tokenResponse.Succeeded)
        {
            return BadRequest(tokenResponse);
        }

        return Ok(tokenResponse);
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout(string refreshToken)
    {
        await _authService.Logout(refreshToken);
        return Ok("Logout Successful");
    }

}
