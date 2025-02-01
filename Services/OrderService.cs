using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Orders;
using LimitlessFit.Models.Requests;
using LimitlessFit.Services.Hubs;

namespace LimitlessFit.Services;

[Authorize]
public class OrderService(
    ApplicationDbContext context,
    IAuthService authService,
    INotificationService notificationService,
    IHubContext<OrderUpdateHub> hubContext) : IOrderService
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

        var totalPrice = Math.Round(
            filteredItems.Sum(item =>
                item.Price * request.Items.First(itemRequest => itemRequest.ItemId == item.Id).Quantity),
            2,
            MidpointRounding.AwayFromZero
        );

        var userId = authService.GetUserIdFromClaims();

        var order = new Order
        {
            UserId = userId,
            Date = DateTime.UtcNow,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending,
            Items = request.Items.Select(itemRequest => new OrderItem
            {
                ItemId = itemRequest.ItemId,
                Quantity = itemRequest.Quantity
            }).ToList()
        };

        context.Orders.Add(order);

        await context.SaveChangesAsync();

        var simplifiedOrder = new
        {
            order.Id,
            order.Status,
            order.Date,
            order.TotalPrice,
            Items = order.Items.Select(item => new
            {
                item.ItemId,
                item.Quantity
            }).ToList()
        };

        await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", simplifiedOrder);

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .Include(order => order.Items)
            .ThenInclude(item => item.Item)
            .FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task<(List<Order> orders, int totalPages)> GetAllOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria)
    {
        var query = context.Orders.AsQueryable();

        if (filterCriteria.StartDate != null)
            query = query.Where(order => order.Date >= filterCriteria.StartDate);

        if (filterCriteria.EndDate != null)
            query = query.Where(order => order.Date <= filterCriteria.EndDate);

        if (filterCriteria.Status != null)
            query = query.Where(order => order.Status == filterCriteria.Status);

        query = query.OrderByDescending(order => order.Date);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / paging.PageSize);

        var orders = await query
            .Include(order => order.User)
            .Include(order => order.Items)
            .ThenInclude(item => item.Item)
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return (orders, totalPages);
    }

    public async Task<(List<Order> orders, int totalPages)> GetMyOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria)
    {
        var userId = authService.GetUserIdFromClaims();
        var query = context.Orders.Where(order => order.UserId == userId);

        if (filterCriteria.StartDate != null)
            query = query.Where(order => order.Date >= filterCriteria.StartDate);

        if (filterCriteria.EndDate != null)
            query = query.Where(order => order.Date <= filterCriteria.EndDate);

        if (filterCriteria.Status != null)
            query = query.Where(order => order.Status == filterCriteria.Status);

        query = query.OrderByDescending(order => order.Date);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / paging.PageSize);

        var orders = await query
            .Include(order => order.Items)
            .ThenInclude(item => item.Item)
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync();

        return (orders, totalPages);
    }

    public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        var order = await context.Orders.FirstOrDefaultAsync(order => order.Id == id);

        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} was not found.");

        order.Status = status;
        context.Orders.Update(order);

        await context.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("ReceiveOrderStatusUpdate", id, status);

        await notificationService.CreateNotificationAsync(
            "orderStatusUpdate",
            new Dictionary<string, object>
            {
                { "id", id },
                { "status", status }
            });
    }
}