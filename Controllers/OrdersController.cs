using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Orders;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrdersService ordersService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await ordersService.CreateOrderAsync(request);

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
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await ordersService.GetOrderByIdAsync(id);

        if (order == null) return NotFound(new { MessageKey = "orderNotFound" });

        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders([FromQuery] PagingRequest paging,
        [FromQuery] OrderFilterCriteria filterCriteria)
    {
        var (orders, totalPages) =
            await ordersService.GetAllOrdersAsync(paging, filterCriteria);

        return Ok(new
        {
            orders,
            totalPages
        });
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] PagingRequest paging,
        [FromQuery] OrderFilterCriteria filterCriteria)
    {
        var (orders, totalPages) =
            await ordersService.GetMyOrdersAsync(paging, filterCriteria);

        return Ok(new
        {
            orders,
            totalPages
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        try
        {
            await ordersService.UpdateOrderStatusAsync(id, status);

            return NoContent();
        }

        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
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