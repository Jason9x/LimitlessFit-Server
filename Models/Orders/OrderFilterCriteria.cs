using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Models.Orders;

[Serializable]
public class OrderFilterCriteria
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public OrderStatus? Status { get; set; }
}