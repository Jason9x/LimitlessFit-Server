using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Models.Dtos.Order;
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
    public async Task<OrderDetailDto> CreateOrderAsync(CreateOrderRequest request)
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

        var orderDetailDto = new OrderDetailDto(
            order.Id,
            order.TotalPrice,
            order.Date,
            order.Status,
            order.Items.Select(item => new OrderItemDto(
                item.ItemId,
                item.Item?.ImageUrl ?? string.Empty,
                item.Item?.NameKey ?? "Unknown",
                item.Quantity,
                item.Item?.Price ?? 0m
            )).ToList()
        );

        await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", orderDetailDto);

        return orderDetailDto;
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(order => order.Id == id)
            .Select(order => new OrderDetailDto(
                order.Id,
                order.TotalPrice,
                order.Date,
                order.Status,
                order.Items.Select(item => item.Item != null
                        ? new OrderItemDto(
                            item.Item.Id,
                            item.Item.ImageUrl ?? string.Empty,
                            item.Item.NameKey ?? "Unknown",
                            item.Quantity,
                            item.Item.Price)
                        : new OrderItemDto(
                            0,
                            string.Empty,
                            "N/A",
                            item.Quantity,
                            0m)
                    )
                    .ToList()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<(List<OrderSummaryDto> orders, int totalPages)> GetAllOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria)
    {
        var query = context.Orders.AsNoTracking().AsQueryable();

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
            .AsNoTracking()
            .Include(order => order.User)
            .Include(order => order.Items)
            .ThenInclude(item => item.Item)
            .Skip((paging.PageNumber - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(order => new OrderSummaryDto(
                order.Id,
                order.User != null ? order.User.Name : "Unknown",
                order.Date,
                order.TotalPrice,
                order.Status,
                order.Items.Select(item => item.Item != null
                        ? new OrderItemSummaryDto(
                            item.Item.Id,
                            item.Item.ImageUrl,
                            item.Item.NameKey ?? string.Empty,
                            item.Item.Price,
                            item.Item.DescriptionKey ?? string.Empty,
                            item.Quantity
                        )
                        : new OrderItemSummaryDto(
                            0,
                            string.Empty,
                            string.Empty,
                            0,
                            string.Empty,
                            item.Quantity))
                    .ToList()
            ))
            .ToListAsync();

        return (orders, totalPages);
    }

    public async Task<(List<MyOrderSummaryDto> orders, int totalPages)> GetMyOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria)
    {
        var userId = authService.GetUserIdFromClaims();
        var query = context.Orders.AsNoTracking().Where(order => order.UserId == userId);

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
            .Select(order => new MyOrderSummaryDto(
                order.Id,
                order.Date,
                order.TotalPrice,
                order.Status
            ))
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
            order.UserId,
            "orderStatusUpdate",
            new Dictionary<string, object>
            {
                { "orderId", id },
                { "status", status }
            });
    }
}