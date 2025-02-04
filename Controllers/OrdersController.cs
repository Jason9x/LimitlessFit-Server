using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Orders;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await orderService.CreateOrderAsync(request);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        catch (Exception exception)
        {
            var errorType = exception switch
            {
                ArgumentException => OrderErrorType.InvalidOrder,
                InvalidOperationException => OrderErrorType.InvalidItems,
                _ => OrderErrorType.OrderCreationFailed
            };

            return GetErrorResponse(errorType, exception.Message);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await orderService.GetOrderByIdAsync(id);

        return order == null
            ? NotFound(new { MessageKey = "orderNotFound" })
            : Ok(order);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] PagingRequest paging,
        [FromQuery] OrderFilterCriteria filterCriteria)
    {
        var (orders, totalPages) = await orderService.GetAllOrdersAsync(paging, filterCriteria);

        return Ok(new { orders, totalPages });
    }

    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] PagingRequest paging,
        [FromQuery] OrderFilterCriteria filterCriteria)
    {
        var (orders, totalPages) = await orderService.GetMyOrdersAsync(paging, filterCriteria);

        return Ok(new { orders, totalPages });
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        await orderService.UpdateOrderStatusAsync(id, status);

        return NoContent();
    }

    private ObjectResult GetErrorResponse(OrderErrorType errorType, string exceptionMessage)
    {
        var response = new { MessageKey = errorType.ToString(), Message = exceptionMessage };

        return errorType switch
        {
            OrderErrorType.InvalidOrder => BadRequest(response),
            OrderErrorType.InvalidItems => BadRequest(response),
            _ => StatusCode(500, new { MessageKey = errorType.ToString(), Message = "An unexpected error occurred." })
        };
    }
}