using System.ComponentModel.DataAnnotations;

namespace LimitlessFit.Models.Requests.Auth;

public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required]
    [DataType(DataType.Password)]
    string Password
);