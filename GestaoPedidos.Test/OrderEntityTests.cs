using GestaoPedidos.Service.Domain.Entities;
using GestaoPedidos.Service.Domain.Enums;
using GestaoPedidos.Service.Domain.Exceptions;

namespace GestaoPedidos.Test
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithValidItems_CreatesSuccessfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem>
            {
                new OrderItem("Product A", 2, 50m),
                new OrderItem("Product B", 1, 100m)
            };

            // Act
            var order = new Order(customerId, items);

            // Assert
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(2, order.Items.Count);
            Assert.NotEqual(DateTime.MinValue, order.CreatedAt);
        }

        [Fact]
        public void Order_WithEmptyItems_ThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem>();

            // Act & Assert
            Assert.Throws<DomainException>(() => new Order(customerId, items));
        }

        [Fact]
        public void Order_WithNullItems_ThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() => new Order(customerId, null));
        }

        [Fact]
        public void Order_CalculatesTotalAmount_Correctly()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem>
            {
                new OrderItem("Product A", 2, 50m),  // 100m
                new OrderItem("Product B", 1, 100m)  // 100m
            };

            // Act
            var order = new Order(customerId, items);

            // Assert
            Assert.Equal(200m, order.TotalAmount);
        }

        [Fact]
        public void Order_Cancel_WithPendingStatus_CancelsSuccessfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem> { new OrderItem("Product", 1, 50m) };
            var order = new Order(customerId, items);

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public void Order_Cancel_WithCancelledStatus_ThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem> { new OrderItem("Product", 1, 50m) };
            var order = new Order(customerId, items);
            order.Cancel();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Cancel());
        }

        [Fact]
        public void Order_CreatedAt_IsCurrentUtcTime()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem> { new OrderItem("Product", 1, 50m) };
            var beforeCreation = DateTime.UtcNow;

            // Act
            var order = new Order(customerId, items);
            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.True(order.CreatedAt >= beforeCreation);
            Assert.True(order.CreatedAt <= afterCreation);
        }

        [Fact]
        public void Order_Items_AreReadOnly()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem> { new OrderItem("Product", 1, 50m) };
            var order = new Order(customerId, items);

            // Act & Assert
            var orderItems = order.Items;
            Assert.IsAssignableFrom<IReadOnlyCollection<OrderItem>>(orderItems);

            // Verify we cannot modify items directly
            var itemsList = orderItems as List<OrderItem>;
            Assert.Null(itemsList); // Should not be castable to List
        }
    }

    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_WithValidData_CreatesSuccessfully()
        {
            // Arrange
            var productName = "Product A";
            var quantity = 5;
            var unitPrice = 50m;

            // Act
            var item = new OrderItem(productName, quantity, unitPrice);

            // Assert
            Assert.Equal(productName, item.ProductName);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(unitPrice, item.UnitPrice);
        }

        [Fact]
        public void OrderItem_CalculatesTotalPrice_Correctly()
        {
            // Arrange
            var item = new OrderItem("Product", 3, 25m);

            // Act
            var totalPrice = item.TotalPrice;

            // Assert
            Assert.Equal(75m, totalPrice);
        }

        [Fact]
        public void OrderItem_WithNegativeQuantity_ThrowsDomainException()
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new OrderItem("Product", -1, 50m));
        }

        [Fact]
        public void OrderItem_WithZeroQuantity_ThrowsDomainException()
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new OrderItem("Product", 0, 50m));
        }
    }
}
