using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Helper;
using TestProject.Helper.Mock;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Model;

namespace TestProject.Repository
{
    public class OrderItemRepositoryTests : RepositoryTestBase
    {
        private readonly OrderItemRepository _orderItemRepository;
        private readonly OnlineStoreDBContext _context;
        public OrderItemRepositoryTests()
        {
            _context = GetDbContext();
            _orderItemRepository = new OrderItemRepository(_context);
        }
        [Fact]
        public async Task GetAllOrderItemsAsync_ShouldReturnAllOrderItems()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            int numberOfOrderItems = 3;
            var orderItems = MockData.CreateMockOrderItems(order, product, numberOfOrderItems);
            await _context.OrderItems.AddRangeAsync(orderItems);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderItemRepository.getAllOrderItemsAsync(new PaginationParameters { PageId = 1, PageSize = orderItems.Count });
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(numberOfOrderItems);
        }
        [Fact]
        public async Task GetAllOrderItemsByOrderIdAsync_ShouldReturnOrderItemsForGivenOrderId()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            var orderItems = MockData.CreateMockOrderItems(order, product, 3);
            await _context.OrderItems.AddRangeAsync(orderItems);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderItemRepository.getAllOrderItemsByOrderIdAsync(order.OrderId,
                new PaginationParameters { PageId = 1, PageSize = orderItems.Count });
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }
        [Fact]
        public async Task GetOrderItemByIdAsync_WithValidId_ShouldReturnOrderItem()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            var orderItem = MockData.CreateMockOrderItem(order, product);
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderItemRepository.getOrderItemByIdAsync(orderItem.OrderItemId);
            // Assert
            result.Should().NotBeNull();
            result.OrderItemId.Should().Be(orderItem.OrderItemId);
            result.productId.Should().Be(orderItem.productId);
        }
        [Fact]
        public async Task GetOrderItemByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _orderItemRepository.getOrderItemByIdAsync(invalidId);
            // Assert
            result.Should().BeNull();
        }
        [Fact]
        public async Task CreateNewOrderItemId_ShouldAddOrderItemToDatabaser()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            await _context.Orders.AddAsync(order);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            var orderItem = MockData.CreateMockOrderItem(order, product);
            await _context.SaveChangesAsync();
            // Act
            await _orderItemRepository.createNewOrderItemAsync(orderItem);
            // Assert
            var result = await _orderItemRepository.getOrderItemByIdAsync(orderItem.OrderItemId);
            result.Should().NotBeNull();
            result.OrderItemId.Should().Be(orderItem.OrderItemId);
            result.productId.Should().Be(orderItem.productId);
        }
        [Fact]
        public async Task DeleteOrderItemByIdAsync_WithValidId_ShouldDeleteOrderItem()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            var orderItem = MockData.CreateMockOrderItem(order, product);
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderItemRepository.deleteOrderItemByIdAsync(orderItem.OrderItemId);
            // Assert
            result.Should().BeTrue();
            var deletedOrderItem = await _orderItemRepository.getOrderItemByIdAsync(orderItem.OrderItemId);
            deletedOrderItem.Should().BeNull();
        }
        [Fact]
        public async Task DeleteOrderItemByIdAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _orderItemRepository.deleteOrderItemByIdAsync(invalidId);
            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task isThereOrderWithIdAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            var orderItem = MockData.CreateMockOrderItem(order, product);
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            // Act
            var result = await _orderItemRepository.isThereOrderItemWithIdAsync(orderItem.OrderItemId);
            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task isThereOrderWithIdAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _orderItemRepository.isThereOrderItemWithIdAsync(invalidId);
            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async Task GetAllOrderItemsAsync_ShouldReturnEmptyList_WhenNoOrderItemsExist()
        {
            // Arrange
            // No order items added to the context
            // Act
            var result = await _orderItemRepository.getAllOrderItemsAsync(new PaginationParameters { PageId = 1, PageSize = 10 });
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task UpdateOrderItemAsync_ShouldUpdateOrderItemInDatabase()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            var product = MockData.CreateMockProduct();
            var orderItem = MockData.CreateMockOrderItem(order, product);
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();

            orderItem.quantity = 5;
            orderItem.price = orderItem.Product.price * orderItem.quantity;
            // Act
            await _orderItemRepository.updateOrderItemAsync(orderItem);

            // Assert
            var updatedOrderItem = await _orderItemRepository.getOrderItemByIdAsync(orderItem.OrderItemId);
            updatedOrderItem.Should().NotBeNull();
            updatedOrderItem.quantity.Should().Be(5);
            updatedOrderItem.price.Should().Be(orderItem.Product.price * 5);
        }
        [Fact]
        public async Task setOrderAndProductInOrderItem_ShouldSetOrderAndProduct()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            await _context.Orders.AddAsync(order);
            var product = MockData.CreateMockProduct();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            var orderItem = MockData.CreateOrderItemWithProductIdAndOrderId(order.OrderId, product.productId);
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            //Act
            await _orderItemRepository.setOrderAndProductInOrderItem(orderItem);
            // Assert
            orderItem = await  _context.OrderItems.Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItem.OrderItemId);
            orderItem.Should().NotBeNull();
            orderItem.Order.Should().NotBeNull();
            orderItem.Product.Should().NotBeNull();
            orderItem.Order.OrderId.Should().Be(order.OrderId);
            orderItem.Product.productId.Should().Be(product.productId);



        }

    } 
}
