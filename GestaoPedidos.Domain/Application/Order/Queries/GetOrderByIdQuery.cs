using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Queries;

public record GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid Id { get; init; }
}