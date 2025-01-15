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

public class UserService(ApplicationDbContext context, IConfiguration configuration) : IUserService
{
    public (RegistrationResult result, string? token) Register(RegisterRequest request)
    {
        var userExists = context.Users.Any(user => user.Email == request.Email);

        if (userExists) return (RegistrationResult.UserAlreadyExists, null);
        
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = HashPassword(request.Password ?? string.Empty)
        };

        context.Users.Add(user);
    
        var saveResult = context.SaveChanges();
        
        if (saveResult == 0) return (RegistrationResult.Failure, null);
        
        var token = JwtTokenHelper.GenerateJwtToken(user, configuration);

        return (RegistrationResult.Success, token);
    }

    public (User? user, string? token) Authenticate(LoginRequest request)
    {
        var user = context.Users.FirstOrDefault(user => user.Email == request.Email);
        
        if (user == null || !Verify(request.Password, user.Password)) return (null, null);

        var token = JwtTokenHelper.GenerateJwtToken(user, configuration);

        return (user, token);
    }
}