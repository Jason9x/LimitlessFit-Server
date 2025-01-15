using static BCrypt.Net.BCrypt;

using LimitlessFit.Data;
using LimitlessFit.Helpers;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace LimitlessFit.Services;

public enum RegistrationResult
{
    Success,
    UserAlreadyExists,
    Failure
}

public class UserService(ApplicationDbContext context, IConfiguration configuration) : IUserService
{
    public async Task<(RegistrationResult result, string? token)> RegisterAsync(RegisterRequest request)
    {
        var userExists = await context.Users.AnyAsync(user => user.Email == request.Email);

        if (userExists) return (RegistrationResult.UserAlreadyExists, null);
        
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = HashPassword(request.Password ?? string.Empty)
        };

        await context.Users.AddAsync(user);
    
        var saveResult = await context.SaveChangesAsync();
        
        if (saveResult == 0) return (RegistrationResult.Failure, null);
        
        var token = JwtTokenHelper.GenerateJwtToken(user, configuration);

        return (RegistrationResult.Success, token);
    }

    public async Task<(User? user, string? token)> Authenticate(LoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
        
        if (user == null || !Verify(request.Password, user.Password)) return (null, null);

        var token = JwtTokenHelper.GenerateJwtToken(user, configuration);

        return (user, token);
    }
}