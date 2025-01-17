using Microsoft.EntityFrameworkCore;

using static BCrypt.Net.BCrypt;

using LimitlessFit.Data;
using LimitlessFit.Helpers;
using LimitlessFit.Interfaces;
using LimitlessFit.Models;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Services;

public enum RegistrationResult
{
    Success,
    UserAlreadyExists,
    Failure
}

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<(RegistrationResult result, string? token)> RegisterAsync(RegisterRequest request)
    {
        var userExists = await context.Users.AnyAsync(user => user.Email == request.Email);

        if (userExists) return (RegistrationResult.UserAlreadyExists, null);

        var tag = await GenerateUniqueTagAsync(request.Name);
        var taggedName = $"{request.Name}#{tag}";
        
        var user = new User
        {
            Name = taggedName,
            Email = request.Email,
            Password = HashPassword(request.Password)
        };

        await context.Users.AddAsync(user);
    
        var saveResult = await context.SaveChangesAsync();
        
        if (saveResult == 0) return (RegistrationResult.Failure, null);
        
        var token = JwtTokenHelper.GenerateJwtToken(user);

        return (RegistrationResult.Success, token);
    }

    public async Task<(User? user, string? token)> Authenticate(LoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null) return (null, null);

        var isPasswordValid = Verify(request.Password, user.Password);

        if (!isPasswordValid) return (null, null);

        var token = JwtTokenHelper.GenerateJwtToken(user);

        return (user, token);
    }

    private async Task<string> GenerateUniqueTagAsync(string name)
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
}
