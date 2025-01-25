using LimitlessFit.Models.Orders;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IOrdersService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> GetOrderByIdAsync(int id);
    Task<(List<Order> Orders, int TotalPages)> GetAllOrdersAsync(PagingRequest request);
    Task<(List<Order> Orders, int TotalPages)> GetMyOrdersAsync(PagingRequest request);
}