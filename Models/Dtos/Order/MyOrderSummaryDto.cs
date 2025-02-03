using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Models.Dtos.Order;

public record MyOrderSummaryDto(
    int Id,
    DateTime Date,
    decimal TotalPrice,
    OrderStatus Status
);