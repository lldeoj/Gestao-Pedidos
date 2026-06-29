using GestaoPedidos.Service.Application.Interfaces;
using GestaoPedidos.Service.Domain.Entities;
using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Commands.Handle;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(item =>
            new OrderItem(item.ProductName, item.Quantity, item.UnitPrice)
        ).ToList();

        var order = new Order(request.CustomerId, items);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}