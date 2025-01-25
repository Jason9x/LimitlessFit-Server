using LimitlessFit.Models.Enums.Auth;
using LimitlessFit.Models.Requests;
using LimitlessFit.Models.Requests.Auth;

namespace LimitlessFit.Interfaces;

public interface IAuthService
{
    Task<(RegistrationResult result, string? token)> RegisterAsync(RegisterRequest request);
    Task<(LoginResult result, string? token)> Login(LoginRequest request);
    int GetUserIdFromClaims();
}