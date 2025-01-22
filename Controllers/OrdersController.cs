using Microsoft.AspNetCore.Mvc;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Order;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Controllers;

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

        if (order == null) return NotFound(new { MessageKey = "OrderNotFound" });

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] PagingRequest request)
    {
        var orders = await ordersService.GetAllOrdersAsync(request);

        return Ok(orders);
    }

    private ObjectResult GetErrorResponse(OrderErrorType errorType, string exceptionMessage)
    {
        var response = new { MessageKey = errorType.ToString(), Message = exceptionMessage };

        return errorType switch
        {
            OrderErrorType.InvalidOrder => BadRequest(response),
            OrderErrorType.InvalidItems => BadRequest(response),
            _ => StatusCode(500,
                new { MessageKey = errorType.ToString(), Message = "An unexpected error occurred." })
        };
    }
}