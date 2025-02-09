namespace LimitlessFit.Models.Enums.Auth;

public enum PasswordResetResult
{
    Success,
    InvalidToken,
    TokenExpired,
    InvalidPassword
}