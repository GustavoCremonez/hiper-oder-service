using System.Text.RegularExpressions;
using Hiper.Domain.Common;
using Hiper.Domain.Enums;
using Hiper.Domain.Exceptions;

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
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(customerName))
            errors["customerName"] = new[] { "Nome do cliente é obrigatório" };
        else if (customerName.Trim().Length < 3)
            errors["customerName"] = new[] { "Nome do cliente deve ter pelo menos 3 caracteres" };
        else if (customerName.Trim().Length > 100)
            errors["customerName"] = new[] { "Nome do cliente não pode exceder 100 caracteres" };

        if (string.IsNullOrWhiteSpace(customerEmail))
            errors["customerEmail"] = new[] { "Email do cliente é obrigatório" };
        else if (!IsValidEmail(customerEmail))
            errors["customerEmail"] = new[] { "Email do cliente é inválido" };

        if (items == null || items.Count == 0)
            errors["items"] = new[] { "Pedido deve conter pelo menos um item" };

        if (errors.Any())
            throw new ValidationException(errors);

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
        {
            var statusMessages = new Dictionary<OrderStatus, string>
            {
                { OrderStatus.Pending, "Pendente" },
                { OrderStatus.Confirmed, "Confirmado" },
                { OrderStatus.Processing, "Processando" },
                { OrderStatus.Completed, "Concluído" },
                { OrderStatus.Cancelled, "Cancelado" }
            };

            throw new BusinessRuleException(
                $"Não é possível alterar o status de '{statusMessages[Status]}' para '{statusMessages[newStatus]}'. " +
                $"Transição inválida conforme as regras de negócio."
            );
        }

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
