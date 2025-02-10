using System.ComponentModel.DataAnnotations;

namespace LimitlessFit.Models;

public class Role
{
    [Key] public int Id { get; init; }

    [Required] [MaxLength(50)] public string? Name { get; init; }
}