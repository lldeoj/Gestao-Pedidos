using GestaoPedidos.Service.Application.Exceptions;
using GestaoPedidos.Service.Application.Interfaces;
using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Commands.Handle;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            throw new NotFoundException($"Order with ID {request.OrderId} not found");

        order.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}