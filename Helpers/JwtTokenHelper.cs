using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LimitlessFit.Models;

namespace LimitlessFit.Helpers
{
    public static class JwtTokenHelper
    {
        private static readonly string? Key = Environment.GetEnvironmentVariable("JWT_KEY");
        private static readonly string? Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        private static readonly string? Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        public static (string AccessToken, string RefreshToken) GenerateTokens(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var keyBytes = Encoding.UTF8.GetBytes(Key ?? string.Empty);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var refreshToken = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return (
                new JwtSecurityTokenHandler().WriteToken(accessToken),
                new JwtSecurityTokenHandler().WriteToken(refreshToken)
            );
        }
    }
}