using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimitlessFit.Models;

public class Notification
{
    [Key] public int Id { get; init; }

    [Required] public int UserId { get; init; }

    [ForeignKey(nameof(UserId))] public User? User { get; init; }

    [Required] [StringLength(100)] public string? MessageKey { get; init; }

    [Required] public bool IsRead { get; init; }

    [Required] public DateTime CreatedAt { get; init; }

    
    [StringLength(4000)]
    public string? AdditionalData { get; set; }
}