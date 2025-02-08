using System.Security.Claims;
using LimitlessFit.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace LimitlessFit.Authorization;

public class DynamicRoleClaimsTransformer(IServiceProvider serviceProvider) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not { IsAuthenticated: true }) return principal;

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) return principal;

        using var scope = serviceProvider.CreateScope();

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var role = await userService.GetUserRoleIdByIdAsync(int.Parse(userId));
        var claims = principal.Claims.Where(claim => claim.Type != ClaimTypes.Role).ToList();

        claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

        var identity = new ClaimsIdentity(claims, "Bearer");

        return new ClaimsPrincipal(identity);
    }
}