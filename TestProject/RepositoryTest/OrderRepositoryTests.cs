using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Helper;
using TestProject.Helper.Mock;

namespace TestProject.Repository
{
    public class OrderRepositoryTests : RepositoryTestBase
    {
        private readonly OrderRepository _orderRepository;
        private readonly OnlineStoreDBContext _context;
        public OrderRepositoryTests()
        {
            _context = GetDbContext();
            _orderRepository = new OrderRepository(_context);
        }
        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            int numberOfOrders = 3;
            var orders = MockData.CreateMockOrders(numberOfOrders);
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.getAllOrdersAsync(new PaginationParameters { PageId = 1, PageSize = orders.Count });

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(numberOfOrders);
        }
        [Fact]
        public async Task GetOrderByIdAsync_WithValidId_ShouldReturnOrder()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderRepository.getOrderByIdAsync(order.OrderId);
            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(order.OrderId);
            result.orderItems.Should().BeEquivalentTo(order.orderItems);
        }
        [Fact]
        public async Task GetOrderByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _orderRepository.getOrderByIdAsync(invalidId);
            // Assert
            result.Should().BeNull();
        }
        [Fact]
        public async Task CreateNewOrderAsync_ShouldAddOrder()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            // Act
            await _orderRepository.createNewOrderAsync(order);
            // Assert
            var result = await _context.Orders.FindAsync(order.OrderId);
            result.Should().NotBeNull();
            result.orderItems.Should().BeEquivalentTo(order.orderItems);
        }
        [Fact]
        public async Task DeleteOrderByIdAsync_WithValidId_ShouldDeleteOrder()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderRepository.deleteOrderWithIdAsync(order.OrderId);
            // Assert
            result.Should().BeTrue();
            var deletedOrder = await _context.Orders.FindAsync(order.OrderId);
            deletedOrder.Should().BeNull();
        }
        [Fact]
        public async Task DeleteOrderByIdAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _orderRepository.deleteOrderWithIdAsync(invalidId);
            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task IsThereOrderWithId_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderRepository.isThereOrderWithIdAsync(order.OrderId);
            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task IsThereOrderWithId_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _orderRepository.isThereOrderWithIdAsync(invalidId);
            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task GetOrdersByUserIdAsync_WithValidUserId_ShouldReturnOrders()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            int numberOfOrders = 3;
            var orders = MockData.CreateMockOrdersForUser(user.userId, numberOfOrders);
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderRepository.getAllOrdersOfUserByIdAsync(user.userId,
                new PaginationParameters { PageId = 1, PageSize = orders.Count });
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(numberOfOrders);
        }
        [Fact]
        public async Task GetOrdersByUserIdAsync_WithInvalidUserId_ShouldReturnEmptyList()
        {
            // Arrange
            var invalidUserId = 999;
            // Act
            var result = await _orderRepository.getAllOrdersOfUserByIdAsync(invalidUserId, new PaginationParameters { PageId = 1 });
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task GetOrdersByUserIdAsync_WithNoOrders_ShouldReturnEmptyList()
        {
            // Arrange
            var user = MockData.CreateMockUser();
            // Act
            var result = await _orderRepository.getAllOrdersOfUserByIdAsync(user.userId, new PaginationParameters { PageId = 1 });
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task UpdateOrder_ShouldUpdateOrderDetails()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            order.userId = 1001;
            // Act
            await _orderRepository.updateOrder(order);
            // Assert
            var updatedOrder = await _context.Orders.FindAsync(order.OrderId);
            updatedOrder.Should().NotBeNull();
            updatedOrder.userId.Should().Be(1001);
        }
        [Fact]
        public async Task getUserIdOfOrder_ShouldReturnUserId_WhenOrderExists()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderRepository.getUserIdOfOrder(order.OrderId);
            // Assert
            result.Should().Be(order.userId);
        }
        [Fact]
        public async Task getUserIdOfOrder_ShouldReturnZero_WhenOrderDoesNotExist()
        {
            // Arrange
            var invalidOrderId = 999;
            // Act
            var result = await _orderRepository.getUserIdOfOrder(invalidOrderId);
            // Assert
            result.Should().Be(0);
        }
        [Fact]
        public async Task setOrderAndProductInOrderItem_ShouldSetOrderAndProduct()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            await _context.Orders.AddAsync(order);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            var orderItem = MockData.CreateMockOrderItemWithSpecificProductAndOrder(product.productId, order.OrderId);
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            // Act
            await _orderRepository.setOrderAndProductInOrderItem(orderItem);
            // Assert
            orderItem = await _context.OrderItems.Include(oi => oi.Order).Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItem.OrderItemId);
            orderItem.Should().NotBeNull();
            orderItem.Product.Should().Be(product);
            orderItem.productId.Should().Be(product.productId);
            orderItem.Order.Should().Be(order);
            orderItem.OrderId.Should().Be(order.OrderId);
        }
        [Fact]
        public async Task setUserInOrder_ShouldSetUserInOrder()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var user = MockData.CreateMockUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            // Act
            await _orderRepository.setUserInOrder(order, user.userId);
            // Assert
            order.User.Should().NotBeNull();
            order.User.userId.Should().Be(user.userId);
            order.User.Should().Be(user);
        }


    }
}
