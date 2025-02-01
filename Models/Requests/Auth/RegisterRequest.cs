using System.ComponentModel.DataAnnotations;

namespace LimitlessFit.Models.Requests.Auth;

public record RegisterRequest(
    [Required]
    [StringLength(50, MinimumLength = 2)]
    string Name,
    [Required]
    [EmailAddress]
    [StringLength(100)]
    string Email,
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    string Password
);