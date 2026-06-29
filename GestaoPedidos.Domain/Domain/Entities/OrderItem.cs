using GestaoPedidos.Service.Domain.Exceptions;

namespace GestaoPedidos.Service.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem() { }

    public OrderItem(string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
        
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProductName))
            throw new DomainException("Product name is required");
            
        if (Quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");
            
        if (UnitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero");
    }
}