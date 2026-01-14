using System.Text.RegularExpressions;
using Hiper.Domain.Common;
using Hiper.Domain.Enums;

namespace Hiper.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerEmail { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public decimal Total => Items.Sum(i => i.Subtotal);
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Order()
    {
        Items = new List<OrderItem>();
    }

    public static Result<Order> Create(string customerName, string customerEmail, List<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            return Result<Order>.Failure("Nome do cliente é obrigatório");

        if (string.IsNullOrWhiteSpace(customerEmail))
            return Result<Order>.Failure("Email do cliente é obrigatório");

        if (!IsValidEmail(customerEmail))
            return Result<Order>.Failure("Email do cliente é inválido");

        if (items == null || items.Count == 0)
            return Result<Order>.Failure("Pedido deve conter pelo menos um item");

        return Result<Order>.Success(new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName.Trim(),
            CustomerEmail = customerEmail.Trim().ToLowerInvariant(),
            Status = OrderStatus.Pending,
            Items = items,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Result UpdateStatus(OrderStatus newStatus)
    {
        if (!IsValidTransition(Status, newStatus))
            return Result.Failure($"Transição de status de {Status} para {newStatus} não é permitida");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private static bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,
            OrderStatus.Confirmed => newStatus is OrderStatus.Processing or OrderStatus.Cancelled,
            OrderStatus.Processing => newStatus is OrderStatus.Completed or OrderStatus.Cancelled,
            OrderStatus.Completed => false,
            OrderStatus.Cancelled => false,
            _ => false
        };
    }

    private static bool IsValidEmail(string email)
    {
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
}
