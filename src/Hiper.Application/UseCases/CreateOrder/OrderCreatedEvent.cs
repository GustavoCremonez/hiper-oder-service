namespace Hiper.Application.UseCases.CreateOrder;

public record OrderCreatedEvent(
    Guid OrderId,
    string CustomerName,
    string CustomerEmail,
    decimal Total,
    DateTime CreatedAt
);
