using Microsoft.AspNetCore.SignalR;
using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Services;

public class OrderStatusHub : Hub
{
    public async Task SendOrderStatusUpdate(int orderId, OrderStatus status)
    {
        await Clients.All.SendAsync("ReceiveOrderStatusUpdate", orderId, status);
    }
}