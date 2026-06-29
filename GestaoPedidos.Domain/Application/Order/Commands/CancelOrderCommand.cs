using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Commands;

public record CancelOrderCommand : IRequest
{
    public Guid OrderId { get; init; }
}