using GestaoPedidos.Service.Application.Models;
using GestaoPedidos.Service.Domain.Entities;

namespace GestaoPedidos.Service.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Order>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}