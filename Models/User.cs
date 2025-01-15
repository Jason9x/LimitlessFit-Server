using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimitlessFit.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } 

    [Required]
    [EmailAddress] 
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    public required string PasswordHash { get; set; } 
}