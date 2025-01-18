namespace LimitlessFit.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item
{
    [Key]
    public int Id { get; init; }

    [Required] 
    [StringLength(255)]
    public string? ImageUrl { get; init; }

    [Required]
    [StringLength(100)]
    public string? NameKey { get; init; }

    [Required]
    [StringLength(100)]
    public string? DescriptionKey { get; init; }

    [Required] 
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; init; }
}
