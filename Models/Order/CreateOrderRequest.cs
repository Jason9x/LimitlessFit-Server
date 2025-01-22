namespace LimitlessFit.Models.Order;

[Serializable]
public class CreateOrderRequest
{
    public string? CustomerName { get; set; }
    public List<OrderItemRequest>? Items { get; set; }
}

[Serializable]
public class OrderItemRequest
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}