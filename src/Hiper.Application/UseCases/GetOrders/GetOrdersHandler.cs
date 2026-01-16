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

    public async Task<PagedResult<OrderDto>> HandleAsync(GetOrdersQuery query)
    {
        var allOrders = await _orderRepository.GetAllAsync();
        var totalCount = allOrders.Count;

        var orders = allOrders
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => o.ToDto())
            .ToList();

        return new PagedResult<OrderDto>
        {
            Items = orders,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
