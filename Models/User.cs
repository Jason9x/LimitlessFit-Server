using System.ComponentModel.DataAnnotations;

namespace LimitlessFit.Models;

public class User
{
    [Key]
    public int? Id { get; init; }

    [Required]
    [MaxLength(255)]
    public string? Name { get; init; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; init; }

    [Required]
    [MaxLength(255)]
    public string? Password { get; init; }
}
