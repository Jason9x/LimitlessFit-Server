using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static BCrypt.Net.BCrypt;
using LimitlessFit.Data;
using LimitlessFit.Helpers;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Enums.Auth;
using LimitlessFit.Models.Requests.Auth;

namespace LimitlessFit.Services;

public partial class AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    public async Task<(LoginResult result, string? token)> Login(LoginRequest request)
    {
        var user = await context.Users.Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Email == request.Email);

        if (user == null) return (LoginResult.UserNotFound, null);

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            return (LoginResult.AccountLocked, null);

        var isPasswordValid = Verify(request.Password, user.Password);

        if (!isPasswordValid)
        {
            await HandleFailedLoginAsync(user);

            return (LoginResult.InvalidPassword, null);
        }

        await ResetFailedLoginAttemptsAsync(user);

        var token = JwtTokenHelper.GenerateJwtToken(user);

        return (LoginResult.Success, token);
    }

    public async Task<(RegistrationResult result, string? token)> RegisterAsync(RegisterRequest request)
    {
        if (!ValidateEmail(request.Email)) return (RegistrationResult.InvalidEmail, null);

        if (!ValidateName(request.Name)) return (RegistrationResult.InvalidName, null);

        if (!ValidatePasswordPolicy(request.Password))
            return (RegistrationResult.InvalidPassword, null);

        var userExists = await context.Users.AnyAsync(user => user.Email == request.Email);

        if (userExists) return (RegistrationResult.UserAlreadyExists, null);

        var tag = await GenerateUniqueTagAsync(request.Name);
        var taggedName = $"{request.Name}#{tag}";
        
        var defaultRole = await context.Roles.FirstOrDefaultAsync(role => role.Name == "User");

        if (defaultRole == null) return (RegistrationResult.Failure, null);

        var user = new User
        {
            Name = taggedName,
            Email = request.Email,
            Password = HashPassword(request.Password),
            RoleId = defaultRole.Id,
            FailedLoginAttempts = 0,
            LockoutEnd = null
        };

        await context.Users.AddAsync(user);

        var saveResult = await context.SaveChangesAsync();

        if (saveResult == 0) return (RegistrationResult.Failure, null);

        var token = JwtTokenHelper.GenerateJwtToken(user);

        return (RegistrationResult.Success, token);
    }

    public int GetUserIdFromClaims()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId)) return userId;

        throw new UnauthorizedAccessException("User not authenticated");
    }

    private async Task<string> GenerateUniqueTagAsync(string? name)
    {
        string tag;
        bool exists;

        do
        {
            tag = Guid.NewGuid().ToString("N")[..8];
            exists = await context.Users.AnyAsync(user => user.Name == $"{name}#{tag}");
        } while (exists);

        return tag;
    }

    private async Task HandleFailedLoginAsync(User user)
    {
        user.FailedLoginAttempts++;

        if (user.FailedLoginAttempts >= MaxFailedAttempts) await LockAccountAsync(user);

        await context.SaveChangesAsync();
    }

    private async Task LockAccountAsync(User user)
    {
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
        user.FailedLoginAttempts = 0;

        await context.SaveChangesAsync();
    }

    private async Task ResetFailedLoginAttemptsAsync(User user)
    {
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;

        await context.SaveChangesAsync();
    }

    private static bool ValidatePasswordPolicy(string password)
    {
        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsDigit) &&
               password.Any(character => !char.IsLetterOrDigit(character));
    }

    private static bool ValidateEmail(string email)
    {
        var emailRegex = EmailRegex();

        return emailRegex.IsMatch(email);
    }

    private static bool ValidateName(string name)
    {
        var nameRegex = NameRegex();

        return nameRegex.IsMatch(name);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9 ]{3,50}$")]
    private static partial Regex NameRegex();
}