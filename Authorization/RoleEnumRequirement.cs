using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Models.Enums;

namespace LimitlessFit.Authorization;

public class RoleEnumRequirement(RoleEnum requiredRole) : IAuthorizationRequirement
{
    public RoleEnum RequiredRole { get; } = requiredRole;
}