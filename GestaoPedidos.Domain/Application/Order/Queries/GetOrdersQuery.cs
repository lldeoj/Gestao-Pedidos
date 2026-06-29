using GestaoPedidos.Service.Application.Models;
using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Queries;

public record GetOrdersQuery : IRequest<PaginatedList<OrderDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public decimal TotalAmount { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}

public record OrderItemDto
{
    public Guid Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
}