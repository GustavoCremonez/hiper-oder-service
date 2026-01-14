using Hiper.Domain.Common;
using Hiper.Domain.Enums;
using Hiper.Domain.Interfaces;

namespace Hiper.Application.UseCases.CancelOrder;

public class CancelOrderHandler
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> HandleAsync(CancelOrderCommand command)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId);

        if (order == null)
            return Result.Failure("Pedido não encontrado");

        var updateResult = order.UpdateStatus(OrderStatus.Cancelled);

        if (updateResult.IsFailure)
            return Result.Failure(updateResult.Error);

        await _orderRepository.UpdateAsync(order);

        return Result.Success();
    }
}
