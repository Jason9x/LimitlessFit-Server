using Microsoft.AspNetCore.Mvc;
using LimitlessFit.Models.Requests;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Auth;
using LimitlessFit.Models.Requests.Auth;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (result, token) = await authService.Login(request);

        return result switch
        {
            LoginResult.Success => Ok(new { Token = token }),
            LoginResult.UserNotFound => NotFound(new { MessageKey = "userNotFound" }),
            LoginResult.InvalidPassword => Unauthorized(new { MessageKey = "invalidPassword" }),
            LoginResult.AccountLocked => StatusCode(403, new { MessageKey = "accountLocked" }),
            _ => StatusCode(500, new { MessageKey = "loginFailed" })
        };
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (result, token) = await authService.RegisterAsync(request);

        return result switch
        {
            RegistrationResult.Success => Ok(new { Token = token }),
            RegistrationResult.UserAlreadyExists => Conflict(new { MessageKey = "userAlreadyExists" }),
            RegistrationResult.InvalidPassword => BadRequest(new { MessageKey = "passwordRequirementsNotMet" }),
            RegistrationResult.InvalidEmail => BadRequest(new { MessageKey = "invalidEmail" }),
            RegistrationResult.InvalidName => BadRequest(new { MessageKey = "invalidName" }),
            _ => StatusCode(500, new { MessageKey = "registrationFailed" })
        };
    }
}