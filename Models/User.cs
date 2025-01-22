using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LimitlessFit.Models;

public class User
{
    [Key] public int? Id { get; init; }

    [Required] [MaxLength(100)] public string? Name { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; init; }

    [Required] [MaxLength(128)] public string? Password { get; init; }

    [Required] public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEnd { get; set; }
}