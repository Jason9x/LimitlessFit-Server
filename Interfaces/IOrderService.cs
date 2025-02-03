using LimitlessFit.Models.Dtos.Order;
using LimitlessFit.Models.Enums.Order;
using LimitlessFit.Models.Orders;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IOrderService
{
    Task<OrderDetailDto> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDetailDto?> GetOrderByIdAsync(int id);

    Task<(List<OrderSummaryDto> orders, int totalPages)> GetAllOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria);

    Task<(List<MyOrderSummaryDto> orders, int totalPages)> GetMyOrdersAsync(PagingRequest paging,
        OrderFilterCriteria filterCriteria);

    Task UpdateOrderStatusAsync(int id, OrderStatus status);
}