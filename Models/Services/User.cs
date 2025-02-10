using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LimitlessFit.Models.Orders;

namespace LimitlessFit.Models;

[Serializable]
public class User
{
    [Key] public int Id { get; init; }

    [Required] [MaxLength(100)] public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; init; }

    [Required] [MaxLength(128)] public string? Password { get; set; }

    [Required] public int RoleId { get; set; }
    [ForeignKey(nameof(RoleId))] public Role? Role { get; init; }

    [Required] public int FailedLoginAttempts { get; set; }

    [MaxLength(200)] public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime? LockoutEnd { get; set; }

    [MaxLength(255)] public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }


    public List<Order> Orders { get; init; } = [];
}