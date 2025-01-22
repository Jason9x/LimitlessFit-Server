using LimitlessFit.Models.Order;
using LimitlessFit.Models.Requests;

namespace LimitlessFit.Interfaces;

public interface IOrdersService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> GetOrderByIdAsync(int id);
    Task<List<Order>> GetAllOrdersAsync(PagingRequest request);
}