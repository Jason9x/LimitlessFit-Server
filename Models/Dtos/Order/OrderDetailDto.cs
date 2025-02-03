using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Models.Dtos.Order;

public record OrderDetailDto(
    int Id,
    decimal TotalPrice,
    DateTime Date,
    OrderStatus Status,
    List<OrderItemDto> Items);

public record OrderItemDto(
    int Id,
    string ImageUrl,
    string NameKey,
    int Quantity,
    decimal Price);