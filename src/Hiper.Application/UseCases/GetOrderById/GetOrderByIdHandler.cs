using Hiper.Application.DTOs;
using Hiper.Application.Mappings;
using Hiper.Domain.Common;
using Hiper.Domain.Exceptions;
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
            throw new NotFoundException("Pedido", query.Id.ToString());

        return Result<OrderDto>.Success(order.ToDto());
    }
}
