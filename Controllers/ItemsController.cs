using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController(IItemsService itemsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetItems([FromQuery] PagingRequest request)
    {
        var items = await itemsService.GetAllItemsAsync(request);
        var totalItems = await itemsService.GetTotalItemsCountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        return Ok(new
        {
            items,
            totalItems,
            totalPages
        });
    }
}