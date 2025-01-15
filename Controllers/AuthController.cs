using LimitlessFit.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LimitlessFit.Services;
using LimitlessFit.Models.Requests;

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

        if (isAnyFieldEmpty) return BadRequest("Name, Email, and Password are required.");
        
        var (registrationResult, token) = await userService.RegisterAsync(request);

        return registrationResult switch
        {
            RegistrationResult.UserAlreadyExists => BadRequest("User already exists."),
            RegistrationResult.Success => Ok(new { Message = "User registered successfully.", Token = token }),
            _ => StatusCode(500, "An error occurred while registering the user.")
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (user, token) = await userService.Authenticate(request);

        if (user == null || token == null) return Unauthorized(new { Message = "Invalid credentials." });

        return Ok(new { Message = "Login successful.", Token = token });
    }
}