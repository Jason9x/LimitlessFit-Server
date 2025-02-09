using Microsoft.AspNetCore.Identity.Data;
using LimitlessFit.Models.Enums.Auth;
using LoginRequest = LimitlessFit.Models.Requests.Auth.LoginRequest;
using RegisterRequest = LimitlessFit.Models.Requests.Auth.RegisterRequest;

namespace LimitlessFit.Interfaces
{
    public interface IAuthService
    {
        Task<(RegistrationResult result, string? accessToken, string? refreshToken)> RegisterAsync(
            RegisterRequest request);

        Task<(LoginResult result, string? accessToken, string? refreshToken)> Login(LoginRequest request);

        Task<ForgotPasswordResult> ForgotPasswordAsync(string email);
        Task<PasswordResetResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string?> RefreshTokenAsync();

        int GetUserIdFromClaims();
    }
}