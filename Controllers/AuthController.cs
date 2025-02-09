using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Auth;
using LoginRequest = LimitlessFit.Models.Requests.Auth.LoginRequest;
using RegisterRequest = LimitlessFit.Models.Requests.Auth.RegisterRequest;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (result, accessToken, refreshToken) = await authService.Login(request);

        return result switch
        {
            LoginResult.Success => Ok(new { AccessToken = accessToken, RefreshToken = refreshToken }),
            LoginResult.UserNotFound => NotFound(new { MessageKey = "userNotFound" }),
            LoginResult.InvalidPassword => Unauthorized(new { MessageKey = "invalidPassword" }),
            LoginResult.AccountLocked => StatusCode(403, new { MessageKey = "accountLocked" }),
            _ => StatusCode(500, new { MessageKey = "loginFailed" })
        };
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (result, accessToken, refreshToken) = await authService.RegisterAsync(request);

        return result switch
        {
            RegistrationResult.Success => Ok(new { AccessToken = accessToken, RefreshToken = refreshToken }),
            RegistrationResult.UserAlreadyExists => Conflict(new { MessageKey = "userAlreadyExists" }),
            RegistrationResult.InvalidPassword => BadRequest(new { MessageKey = "passwordRequirementsNotMet" }),
            RegistrationResult.InvalidEmail => BadRequest(new { MessageKey = "invalidEmail" }),
            RegistrationResult.InvalidName => BadRequest(new { MessageKey = "invalidName" }),
            _ => StatusCode(500, new { MessageKey = "registrationFailed" })
        };
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await authService.ForgotPasswordAsync(request.Email);

        return result switch
        {
            ForgotPasswordResult.EmailSent => Ok(new { MessageKey = "passwordResetEmailSent" }),
            ForgotPasswordResult.UserNotFound => BadRequest(new { MessageKey = "userNotFound" }),
            _ => StatusCode(500, new { MessageKey = "passwordResetFailed" })
        };
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request);

        return result switch
        {
            PasswordResetResult.Success => Ok(new { MessageKey = "passwordResetSuccessful" }),
            PasswordResetResult.InvalidToken => BadRequest(new { MessageKey = "invalidResetToken" }),
            PasswordResetResult.TokenExpired => BadRequest(new { MessageKey = "resetTokenExpired" }),
            PasswordResetResult.InvalidPassword => BadRequest(new { MessageKey = "passwordRequirementsNotMet" }),
            _ => StatusCode(500, new { MessageKey = "passwordResetFailed" })
        };
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken()
    {
        var accessToken = await authService.RefreshTokenAsync();

        if (accessToken == null) return Unauthorized();

        return Ok(new { AccessToken = accessToken });
    }
}