using Hiper.Application.Interfaces;
using Hiper.Domain.Common;
using Hiper.Domain.Entities;
using Hiper.Domain.Interfaces;

namespace Hiper.Application.UseCases.CreateOrder;

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _messagePublisher;

    public CreateOrderHandler(IOrderRepository orderRepository, IMessagePublisher messagePublisher)
    {
        _orderRepository = orderRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<Result<Guid>> HandleAsync(CreateOrderCommand command)
    {
        var itemsResult = new List<OrderItem>();

        foreach (var itemCommand in command.Items)
        {
            var itemResult = OrderItem.Create(
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            );

            if (itemResult.IsFailure)
                return Result<Guid>.Failure(itemResult.Error);

            itemsResult.Add(itemResult.Value);
        }

        var orderResult = Order.Create(
            command.CustomerName,
            command.CustomerEmail,
            itemsResult
        );

        if (orderResult.IsFailure)
            return Result<Guid>.Failure(orderResult.Error);

        var order = orderResult.Value;
        await _orderRepository.AddAsync(order);

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.Total,
            order.CreatedAt
        );

        await _messagePublisher.PublishAsync(orderCreatedEvent, "order.created");

        return Result<Guid>.Success(order.Id);
    }
}
