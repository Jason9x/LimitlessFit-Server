using LimitlessFit.Models.Enums;

namespace LimitlessFit.Models.Dtos;

public record UserDto(
    int Id,
    string Name,
    string Email,
    RoleEnum RoleId);