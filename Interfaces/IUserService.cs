using LimitlessFit.Models;
using LimitlessFit.Models.Requests;
using LimitlessFit.Services;

namespace LimitlessFit.Interfaces;

public interface IUserService
{
    (RegistrationResult result, string? token) Register(RegisterRequest request);
    (User? user, string? token) Authenticate(LoginRequest request);
}