using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ItemsController(IItemService itemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetItems([FromQuery] PagingRequest request)
    {
        var (items, totalPages) = await itemService.GetAllItemsAsync(request);

        return Ok(new
        {
            items,
            totalPages
        });
    }
}