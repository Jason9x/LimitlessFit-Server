using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using LimitlessFit.Services;

namespace LimitlessFit.Interfaces;

public interface IAuthService
{
    Task<(RegistrationResult result, string? token)> RegisterAsync(RegisterRequest request);
    Task<(User? user, string? token)> Authenticate(LoginRequest request);
}