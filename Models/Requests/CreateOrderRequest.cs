using System.Text.Json.Serialization;

namespace LimitlessFit.Models.Requests;

public record CreateOrderRequest(List<OrderItemRequest>? Items);

[JsonSerializable(typeof(OrderItemRequest))]
public record OrderItemRequest(int ItemId, int Quantity);