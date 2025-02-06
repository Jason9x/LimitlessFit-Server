using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimitlessFit.Models.Orders;

public class OrderItem
{
    [Key] public int Id { get; init; }

    [Required] public int OrderId { get; init; }

    [ForeignKey(nameof(OrderId))] public Order? Order { get; init; }

    [Required] public int ItemId { get; init; }
    [ForeignKey(nameof(ItemId))] public Item? Item { get; init; }

    [Required] public int Quantity { get; init; }
}