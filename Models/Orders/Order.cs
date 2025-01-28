using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LimitlessFit.Models.Enums.Order;

namespace LimitlessFit.Models.Orders;

public class Order
{
    [Key] public int Id { get; init; }

    [Required] public int UserId { get; init; }

    [ForeignKey(nameof(UserId))] public User? User { get; init; }

    [Required] public DateTime Date { get; init; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; init; }

    [Required] public OrderStatus Status { get; set; }

    public List<OrderItem> Items { get; init; } = [];
}