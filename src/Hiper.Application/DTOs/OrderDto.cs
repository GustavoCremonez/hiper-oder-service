namespace Hiper.Application.DTOs;

public record OrderDto(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    string Status,
    List<OrderItemDto> Items,
    decimal Total,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
