using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        UpdateUserRole(int id, [FromBody] int role)
    {
        await userService.UpdateUserRoleAsync(id, role);

        return NoContent();
    }
}