using Microsoft.AspNetCore.Mvc;

namespace LimitlessFit.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("text/plain")]
public class PingController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}