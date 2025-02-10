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

public class OrderService(
    ApplicationDbContext context,
    IAuthService authService,
    IUserService userService,
    INotificationService notificationService,
    IHubContext<OrderUpdateHub> hubContext) : IOrderService
{
    public async Task<OrderSummaryDto> CreateOrderAsync(CreateOrderRequest request)
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
        var username = await userService.GetUserNameByIdAsync(userId);

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

        var orderSummaryDto = new OrderSummaryDto(
            order.Id,
            username,
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
        );

        await hubContext.Clients.All.SendAsync("AddNewOrder", orderSummaryDto);

        return orderSummaryDto;
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
                            item.Item.NameKey ?? string.Empty,
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
                order.User != null ? order.User.Name : string.Empty,
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

    public async Task<OrderStatsDto> GetOrderStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var deliveredToday = await context.Orders.CountAsync(order =>
            order.Status == OrderStatus.Delivered &&
            order.Date >= today && order.Date < tomorrow);

        var pendingOrders = await context.Orders.CountAsync(order =>
            order.Status == OrderStatus.Pending);

        var shippingOrders = await context.Orders.CountAsync(order =>
            order.Status == OrderStatus.Shipping);

        return new OrderStatsDto(
            deliveredToday,
            pendingOrders,
            shippingOrders
        );
    }

    public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        var order = await context.Orders.FirstOrDefaultAsync(order => order.Id == id);

        if (order == null)
            throw new KeyNotFoundException($"Order with Id {id} was not found.");

        var previousStatus = order.Status;
        var date = order.Date;

        order.Status = status;

        context.Orders.Update(order);

        await context.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("ReceivedOrderStatusUpdate", new { id, previousStatus, status, date });

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