using Microsoft.AspNetCore.Mvc;

using LimitlessFit.Interfaces;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(IItemsService itemsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await itemsService.GetAllItemsAsync();
        
        return Ok(items);
    }
}