using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Models.Orders;

public record OrderFilterCriteria(
    DateTime? StartDate,
    DateTime? EndDate,
    OrderStatus? Status
);