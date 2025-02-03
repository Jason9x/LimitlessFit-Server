using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Models.Dtos.Order;

public record OrderSummaryDto(
    int Id,
    string Username,
    DateTime Date,
    decimal TotalPrice,
    OrderStatus Status,
    List<OrderItemSummaryDto> Items);

public record OrderItemSummaryDto(
    int Id,
    string? ImageUrl,
    string NameKey,
    decimal Price,
    string DescriptionKey,
    int Quantity);