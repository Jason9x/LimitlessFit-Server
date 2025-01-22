using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Order;
using LimitlessFit.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace LimitlessFit.Services;

[Authorize]
public class OrdersService(ApplicationDbContext context) : IOrdersService
{
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("Order must include at least one item.");

        var items = await context.Items.ToListAsync();
        var itemIds = request.Items.Select(item => item.ItemId).ToList();
        var filteredItems = items.Where(item => itemIds.Contains(item.Id)).ToList(); 

        if (filteredItems.Count != itemIds.Count)
            throw new InvalidOperationException("Some items do not exist.");

        var totalPrice = filteredItems.Sum(item =>
            item.Price * request.Items.First(itemRequest => itemRequest.ItemId == item.Id).Quantity);

        var order = new Order
        {
            CustomerName = request.CustomerName,
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending,
            OrderItems = request.Items.Select(itemRequest => new OrderItem
            {
                ItemId = itemRequest.ItemId,
                Quantity = itemRequest.Quantity
            }).ToList()
        };

        context.Orders.Add(order);

        await context.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Item)
            .FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task<List<Order>> GetAllOrdersAsync(PagingRequest request)
    {
        return await context.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Item)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
    }
}