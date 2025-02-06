using LimitlessFit.Models.Enums.Auth;
using LimitlessFit.Models.Requests.Auth;

namespace LimitlessFit.Interfaces
{
    public interface IAuthService
    {
        Task<(RegistrationResult result, string? accessToken, string? refreshToken)> RegisterAsync(
            RegisterRequest request);

        Task<(LoginResult result, string? accessToken, string? refreshToken)> Login(LoginRequest request);
        Task<string?> RefreshTokenAsync();
        int GetUserIdFromClaims();
    }
}