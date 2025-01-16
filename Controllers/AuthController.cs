using Microsoft.AspNetCore.Mvc;

using LimitlessFit.Services;
using LimitlessFit.Models.Requests;
using LimitlessFit.Interfaces;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var isAnyFieldEmpty = string.IsNullOrEmpty(request.Name) ||
                              string.IsNullOrEmpty(request.Email) ||
                              string.IsNullOrEmpty(request.Password);

        if (isAnyFieldEmpty) return BadRequest(new { MessageKey = "fieldsRequired" });

        var (registrationResult, token) = await userService.RegisterAsync(request);

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
        var (user, token) = await userService.Authenticate(request);

        if (user == null || token == null) return Unauthorized(new { MessageKey = "invalidCredentials" });

        return Ok(new { MessageKey = "successfulLogin", Token = token });
    }
}