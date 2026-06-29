using GestaoPedidos.Service.Domain.Entities;
using GestaoPedidos.Service.Infrastructure.Data;
using GestaoPedidos.Service.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestaoPedidos.Test
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly OrderDbContext _context;
        private readonly OrderRepository _repository;

        public OrderRepositoryTests()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            _repository = new OrderRepository(_context);
        }

        [Fact]
        public async Task AddAsync_WithValidOrder_AddsOrderToDatabase()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem>
            {
                new OrderItem("Product A", 2, 50m),
                new OrderItem("Product B", 1, 100m)
            };
            var order = new Order(customerId, items);

            // Act
            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            // Assert
            var addedOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            Assert.NotNull(addedOrder);
            Assert.Equal(customerId, addedOrder.CustomerId);
            Assert.Equal(2, addedOrder.Items.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsOrder()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem> { new OrderItem("Product", 1, 50m) };
            var order = new Order(customerId, items);

            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(customerId, result.CustomerId);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_IncludesItems()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<OrderItem>
            {
                new OrderItem("Product A", 2, 50m),
                new OrderItem("Product B", 1, 100m)
            };
            var order = new Order(customerId, items);

            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.True(result.Items.Any(i => i.ProductName == "Product A"));
        }

        [Fact]
        public async Task GetPaginatedAsync_WithPage1_ReturnsFirstPage()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 1", 1, 50m) }),
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 2", 1, 60m) }),
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 3", 1, 70m) })
            };

            foreach (var order in orders)
            {
                await _repository.AddAsync(order);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPaginatedAsync(1, 2);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.PageSize);
        }

        [Fact]
        public async Task GetPaginatedAsync_WithPage2_ReturnsSecondPage()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 1", 1, 50m) }),
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 2", 1, 60m) }),
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 3", 1, 70m) })
            };

            foreach (var order in orders)
            {
                await _repository.AddAsync(order);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPaginatedAsync(2, 2);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.Items.Count);
            Assert.Equal(2, result.Page);
        }

        [Fact]
        public async Task GetPaginatedAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetPaginatedAsync(1, 10);

            // Assert
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetPaginatedAsync_OrderedByCreatedAtDescending()
        {
            // Arrange
            var order1 = new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 1", 1, 50m) });
            await Task.Delay(100);
            var order2 = new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 2", 1, 60m) });

            await _repository.AddAsync(order1);
            await _repository.AddAsync(order2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPaginatedAsync(1, 10);

            // Assert
            // Most recent order should be first
            Assert.Equal(order2.Id, result.Items.First().Id);
            Assert.Equal(order1.Id, result.Items.Last().Id);
        }

        [Fact]
        public async Task GetPaginatedAsync_IncludesItems()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem("Product A", 2, 50m),
                new OrderItem("Product B", 1, 100m)
            };
            var order = new Order(Guid.NewGuid(), items);

            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPaginatedAsync(1, 10);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(2, result.Items.First().Items.Count);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
