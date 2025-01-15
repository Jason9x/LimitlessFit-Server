using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimitlessFit.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 

    [Required]
    public string Name { get; set; } 

    [Required]
    [EmailAddress] 
    public string Email { get; set; }

    [Required]
    public required string Password { get; set; } 
}