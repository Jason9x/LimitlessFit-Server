namespace LimitlessFit.Models.Order;

using LimitlessFit.Models.Enums.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    [Key] public int Id { get; init; }

    [Required] [StringLength(100)] public string? CustomerName { get; init; }

    [Required] public DateTime? Date { get; init; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; init; }

    [Required] public OrderStatus Status { get; init; }

    public List<OrderItem> Items { get; init; } = [];
}