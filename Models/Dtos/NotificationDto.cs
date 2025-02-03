namespace LimitlessFit.Models.Dtos;

public record NotificationDto(
    int Id,
    string MessageKey,
    DateTime CreatedAt,
    bool IsRead,
    string? AdditionalData);