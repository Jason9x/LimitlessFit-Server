using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using LimitlessFit.Models;

namespace LimitlessFit.Helpers
{
    public static class JwtTokenHelper
    {
        public static string GenerateJwtToken(User user, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var keyBytes = Encoding.UTF8.GetBytes(key ?? string.Empty);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiry time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Return the generated JWT token as a string
        }
    }
}