using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Orders;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> GetOrderByIdAsync(int id);
    
    Task<(List<Order> orders, int totalPages)> GetMyOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria);
    
    Task<(List<Order> orders, int totalPages)> GetAllOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria);

    Task UpdateOrderStatusAsync(int id, OrderStatus status);
}