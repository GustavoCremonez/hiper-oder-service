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
        var (orders, totalCount) = await _orderRepository.GetPagedAsync(query.Page, query.PageSize);

        var orderDtos = orders
            .Select(o => o.ToDto())
            .ToList();

        return new PagedResult<OrderDto>
        {
            Items = orderDtos,
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
