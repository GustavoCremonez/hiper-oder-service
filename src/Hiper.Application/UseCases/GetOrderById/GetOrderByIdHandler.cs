using Hiper.Application.DTOs;
using Hiper.Application.Mappings;
using Hiper.Domain.Common;
using Hiper.Domain.Interfaces;

namespace Hiper.Application.UseCases.GetOrderById;

public class GetOrderByIdHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> HandleAsync(GetOrderByIdQuery query)
    {
        var order = await _orderRepository.GetByIdAsync(query.Id);

        if (order == null)
            return Result<OrderDto>.Failure("Pedido não encontrado");

        return Result<OrderDto>.Success(order.ToDto());
    }
}
