using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LimitlessFit.Authorization;

public class RoleEnumHandler : AuthorizationHandler<RoleEnumRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleEnumRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);

        if (roleClaim == null || !int.TryParse(roleClaim.Value, out var userRole)) return Task.CompletedTask;

        if (userRole == (int)requirement.RequiredRole) context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}