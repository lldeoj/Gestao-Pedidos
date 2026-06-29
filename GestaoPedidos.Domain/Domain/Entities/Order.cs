using GestaoPedidos.Service.Domain.Enums;
using GestaoPedidos.Service.Domain.Exceptions;

namespace GestaoPedidos.Service.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();
    
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    
    public decimal TotalAmount => _items.Sum(item => item.TotalPrice);

    private Order() { }

    public Order(Guid customerId, List<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        if (items == null || !items.Any())
            throw new DomainException("Order must have at least one item");
            
        _items.AddRange(items);
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be cancelled");
            
        Status = OrderStatus.Cancelled;
    }
}