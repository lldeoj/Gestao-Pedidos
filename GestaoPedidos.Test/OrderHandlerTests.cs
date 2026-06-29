using GestaoPedidos.Service.Application.Interfaces;
using GestaoPedidos.Service.Application.Models;
using GestaoPedidos.Service.Application.Orders.Commands;
using GestaoPedidos.Service.Application.Orders.Commands.Handle;
using GestaoPedidos.Service.Application.Orders.Queries;
using GestaoPedidos.Service.Application.Orders.Queries.GetOrders;
using GestaoPedidos.Service.Domain.Entities;
using Moq;

namespace GestaoPedidos.Test
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_CreatesOrderAndReturnsId()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductName = "Product 1", Quantity = 2, UnitPrice = 100m },
                new CreateOrderItemDto { ProductName = "Product 2", Quantity = 1, UnitPrice = 50m }
            };

            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                Items = items
            };

            _mockOrderRepository
                .Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);

            _mockOrderRepository.Verify(
                x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockUnitOfWork.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithMultipleItems_CreatesOrderWithAllItems()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var items = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductName = "Product A", Quantity = 3, UnitPrice = 25m },
                new CreateOrderItemDto { ProductName = "Product B", Quantity = 2, UnitPrice = 50m },
                new CreateOrderItemDto { ProductName = "Product C", Quantity = 1, UnitPrice = 100m }
            };

            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                Items = items
            };

            Order capturedOrder = null;
            _mockOrderRepository
                .Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Callback<Order, CancellationToken>((order, ct) => capturedOrder = order)
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedOrder);
            Assert.Equal(customerId, capturedOrder.CustomerId);
            Assert.Equal(3, capturedOrder.Items.Count);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductName = "Product", Quantity = 1, UnitPrice = 100m }
                }
            };

            _mockOrderRepository
                .Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }

    public class GetOrdersQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly GetOrdersQueryHandler _handler;

        public GetOrdersQueryHandlerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _handler = new GetOrdersQueryHandler(_mockOrderRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidQuery_ReturnsOrdersList()
        {
            // Arrange
            var items = new List<OrderItem> { new OrderItem("Product", 1, 50m) };
            var orders = new List<Order>
            {
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 1", 2, 50m) }),
                new Order(Guid.NewGuid(), new List<OrderItem> { new OrderItem("Product 2", 1, 100m) })
            };

            var paginatedList = new PaginatedList<Order>(orders, 2, 1, 10);

            _mockOrderRepository
                .Setup(x => x.GetPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedList);

            var query = new GetOrdersQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task Handle_WithEmptyResult_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new PaginatedList<Order>(new List<Order>(), 0, 1, 10);

            _mockOrderRepository
                .Setup(x => x.GetPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            var query = new GetOrdersQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task Handle_WithOrdersContainingItems_MapsItemsCorrectly()
        {
            // Arrange
            var orderItems = new List<OrderItem>
            {
                new OrderItem("Product 1", 2, 50m),
                new OrderItem("Product 2", 1, 100m)
            };

            var order = new Order(Guid.NewGuid(), orderItems);
            var orders = new List<Order> { order };

            var paginatedList = new PaginatedList<Order>(orders, 1, 1, 10);

            _mockOrderRepository
                .Setup(x => x.GetPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedList);

            var query = new GetOrdersQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result.Items);
            var returnedOrder = result.Items.First();
            Assert.Equal(2, returnedOrder.Items.Count);
            Assert.Equal("Product 1", returnedOrder.Items[0].ProductName);
            Assert.Equal("Product 2", returnedOrder.Items[1].ProductName);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockOrderRepository
                .Setup(x => x.GetPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            var query = new GetOrdersQuery { Page = 1, PageSize = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
