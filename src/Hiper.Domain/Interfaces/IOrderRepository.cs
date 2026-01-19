using Hiper.Domain.Entities;

namespace Hiper.Domain.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<(List<Order> Orders, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
}
