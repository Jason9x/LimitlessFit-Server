using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] UserSearchRequest request)
    {
        var (users, totalPages) = await userService.GetUsersAsync(request);

        return Ok(new { users, totalPages });
    }

    [HttpPatch("{id:int}/role")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        UpdateUserRole(int id, [FromBody] RoleEnum role)
    {
        await userService.UpdateUserRoleAsync(id, role);

        return NoContent();
    }

    [HttpGet("{id:int}/role")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserRole(int id)
    {
        var roleId = await userService.GetUserRoleIdByIdAsync(id);

        return Ok(roleId);
    }
}