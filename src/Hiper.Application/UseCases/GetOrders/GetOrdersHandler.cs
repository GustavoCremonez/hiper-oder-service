using Hiper.Application.DTOs;
using Hiper.Application.Mappings;
using Hiper.Domain.Interfaces;

namespace Hiper.Application.UseCases.GetOrders;

public class GetOrdersHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<OrderDto>> HandleAsync(GetOrdersQuery query)
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(o => o.ToDto()).ToList();
    }
}
