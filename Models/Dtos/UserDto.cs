namespace LimitlessFit.Models.Dtos;

public record UserDto(
    int Id,
    string Name,
    string Email,
    int RoleId);