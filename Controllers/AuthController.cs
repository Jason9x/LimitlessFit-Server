using Microsoft.AspNetCore.Mvc;

using LimitlessFit.Services;
using LimitlessFit.Models.Requests;
using LimitlessFit.Interfaces;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (registrationResult, token) = await authService.RegisterAsync(request);

        return registrationResult switch
        {
            RegistrationResult.UserAlreadyExists => BadRequest(new { MessageKey = "userAlreadyExists" }),
            RegistrationResult.Success => Ok(new { MessageKey = "successfulAccountCreation", Token = token }),
            _ => StatusCode(500, new { MessageKey = "error" })
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (user, token) = await authService.Authenticate(request);

        if (user == null || token == null) return Unauthorized(new { MessageKey = "invalidCredentials" });

        return Ok(new { MessageKey = "successfulLogin", Token = token });
    }
}