using Hiper.Domain.Common;

namespace Hiper.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal => Quantity * UnitPrice;

    private OrderItem() { }

    public static Result<OrderItem> Create(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            return Result<OrderItem>.Failure("Nome do produto é obrigatório");

        if (quantity < 1)
            return Result<OrderItem>.Failure("Quantidade deve ser maior ou igual a 1");

        if (unitPrice <= 0)
            return Result<OrderItem>.Failure("Preço unitário deve ser maior que zero");

        return Result<OrderItem>.Success(new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductName = productName.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice
        });
    }
}
