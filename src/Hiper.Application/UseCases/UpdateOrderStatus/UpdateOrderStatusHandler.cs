using Hiper.Application.DTOs;
using Hiper.Application.Mappings;
using Hiper.Domain.Common;
using Hiper.Domain.Interfaces;

namespace Hiper.Application.UseCases.UpdateOrderStatus;

public class UpdateOrderStatusHandler
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> HandleAsync(UpdateOrderStatusCommand command)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId);

        if (order == null)
            return Result<OrderDto>.Failure("Pedido não encontrado");

        var updateResult = order.UpdateStatus(command.NewStatus);

        if (updateResult.IsFailure)
            return Result<OrderDto>.Failure(updateResult.Error);

        await _orderRepository.UpdateAsync(order);

        return Result<OrderDto>.Success(order.ToDto());
    }
}
