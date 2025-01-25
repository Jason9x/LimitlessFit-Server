namespace LimitlessFit.Models.Requests;

[Serializable]
public class CreateOrderRequest
{
    public List<OrderItemRequest>? Items { get; set; }
}

[Serializable]
public class OrderItemRequest
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}