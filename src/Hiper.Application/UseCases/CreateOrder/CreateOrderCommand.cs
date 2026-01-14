namespace Hiper.Application.UseCases.CreateOrder;

public record CreateOrderCommand(
    string CustomerName,
    string CustomerEmail,
    List<CreateOrderItemCommand> Items
);

public record CreateOrderItemCommand(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
