using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Commands;

public record CreateOrderCommand : IRequest<Guid>
{
    public Guid CustomerId { get; init; }
    public List<CreateOrderItemDto> Items { get; init; } = new();
}

public record CreateOrderItemDto
{
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}