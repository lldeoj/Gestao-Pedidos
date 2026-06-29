using GestaoPedidos.Service.Application.Interfaces;
using GestaoPedidos.Service.Application.Models;
using MediatR;

namespace GestaoPedidos.Service.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);

        var orderDtos = orders.Items.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        }).ToList();

        return new PaginatedList<OrderDto>(
            orderDtos,
            orders.TotalCount,
            orders.Page,
            orders.PageSize);
    }
}