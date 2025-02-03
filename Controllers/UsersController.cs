using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
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
}