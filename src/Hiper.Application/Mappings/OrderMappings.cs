using Hiper.Application.DTOs;
using Hiper.Domain.Entities;

namespace Hiper.Application.Mappings;

public static class OrderMappings
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.Status.ToString(),
            order.Items.Select(i => i.ToDto()).ToList(),
            order.Total,
            order.CreatedAt,
            order.UpdatedAt
        );
    }

    public static OrderItemDto ToDto(this OrderItem item)
    {
        return new OrderItemDto(
            item.Id,
            item.ProductName,
            item.Quantity,
            item.UnitPrice,
            item.Subtotal
        );
    }
}
