using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController(IItemsService itemsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetItems(int pageNumber = 1, int pageSize = 10)
    {
        var items = await itemsService.GetAllItemsAsync(pageNumber, pageSize);
        var totalItems = await itemsService.GetTotalItemsCountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return Ok(new
        {
            items,
            pageNumber,
            pageSize,
            totalItems,
            totalPages
        });
    }
}