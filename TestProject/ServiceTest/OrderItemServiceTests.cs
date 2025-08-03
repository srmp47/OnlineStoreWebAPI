using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestProject.Helper.Mock;
using TestProject.Helper;
using Xunit;

namespace TestProject.ServiceTest
{
    public class OrderItemServiceTests : ServiceTestBase
    {
        private readonly Mock<IOrderItemRepository> _mockRepo;
        private readonly OrderItemService _orderItemService;

        public OrderItemServiceTests()
        {
            _mockRepo = CreateMock<IOrderItemRepository>();
            _orderItemService = new OrderItemService(_mockRepo.Object, Mapper);
        }

        [Fact]
        public async Task ChangeQuantityByOrderItemId_ShouldUpdateQuantityAndRecalculate()
        {
            // Arrange
            int orderItemId = 1;
            int newQuantity = 5;

            var product = MockData.CreateMockProductWithSpecificPrice(10);

            var order = new Order { totalAmount = 100 };

            var orderItem = new OrderItem
            {
                OrderItemId = orderItemId,
                quantity = 2,
                price = 20, 
                Product = product,
                Order = order
            };

            _mockRepo.Setup(r => r.getOrderItemByIdAsync(orderItemId)).ReturnsAsync(orderItem);
            _mockRepo.Setup(r => r.updateOrderItemAsync(It.IsAny<OrderItem>())).ReturnsAsync((OrderItem oi) => oi);

            // Act
            var result = await _orderItemService.changeQuantityByOrderItemId(orderItemId, newQuantity);

            // Assert
            result.quantity.Should().Be(newQuantity);
            result.price.Should().Be(50); // 5 * 10
            order.totalAmount.Should().Be(130); // 100 - 20 + 50
            _mockRepo.Verify(r => r.updateOrderItemAsync(orderItem), Times.Once);
        }

        [Fact]
        public async Task CreateNewOrderItemAsync_ShouldCallRepository()
        {
            // Arrange
            var orderItem = MockData.CreateMockOrderItemWithSpecificProductAndOrder(1, 1);
            _mockRepo.Setup(r => r.createNewOrderItemAsync(orderItem)).Returns(Task.CompletedTask);

            // Act
            var result = await _orderItemService.createNewOrderItemAsync(orderItem);

            // Assert
            result.Should().Be(orderItem);
            _mockRepo.Verify(r => r.createNewOrderItemAsync(orderItem), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderItemByIdAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            int orderItemId = 1;
            _mockRepo.Setup(r => r.deleteOrderItemByIdAsync(orderItemId)).ReturnsAsync(true);

            // Act
            var result = await _orderItemService.deleteOrderItemByIdAsync(orderItemId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllOrderItemsAsync_ShouldReturnItems()
        {
            // Arrange
            var pagination = new PaginationParameters { PageId = 1 , PageSize = 10};
            var orderItems = new List<OrderItem>
            {
                MockData.CreateMockOrderItemWithSpecificOrder(1)
            };

            _mockRepo.Setup(r => r.getAllOrderItemsAsync(pagination)).ReturnsAsync(orderItems);

            // Act
            var result = await _orderItemService.getAllOrderItemsAsync(pagination);

            // Assert
            result.Should().BeEquivalentTo(orderItems);
        }

        [Fact]
        public async Task GetOrderItemByOrderItemId_ShouldReturnItem()
        {
            // Arrange
            int orderItemId = 1;
            var orderItem = MockData.CreateMockOrderItemWithSpecificOrder(1);
            _mockRepo.Setup(r => r.getOrderItemByIdAsync(orderItemId)).ReturnsAsync(orderItem);

            // Act
            var result = await _orderItemService.getOrderItemByOrderItemId(orderItemId);

            // Assert
            result.Should().Be(orderItem);
        }

        [Fact]
        public async Task IsThereOrderItemById_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            int orderItemId = 1;
            _mockRepo.Setup(r => r.isThereOrderItemWithIdAsync(orderItemId)).ReturnsAsync(true);

            // Act
            var result = await _orderItemService.isThereOrderItemById(orderItemId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SetOrderAndProductInOrderItem_ShouldCallRepository()
        {
            // Arrange
            var orderItem = MockData.CreateMockOrderItemWithSpecificProductAndOrder(1, 1);
            _mockRepo.Setup(r => r.setOrderAndProductInOrderItem(orderItem)).Returns(Task.CompletedTask);

            // Act
            await _orderItemService.setOrderAndProductInOrderItem(orderItem);

            // Assert
            _mockRepo.Verify(r => r.setOrderAndProductInOrderItem(orderItem), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderItemAsync_ShouldCallRepository()
        {
            // Arrange
            var orderItem = MockData.CreateMockOrderItemWithSpecificOrder(1);
            _mockRepo.Setup(r => r.updateOrderItemAsync(orderItem)).ReturnsAsync(orderItem);

            // Act
            var result = await _orderItemService.updateOrderItemAsync(orderItem);

            // Assert
            result.Should().Be(orderItem);
        }

        [Fact]
        public async Task GetAllOrderItemsByOrderIdAsync_ShouldReturnFilteredItems()
        {
            // Arrange
            int orderId = 1;
            var pagination = new PaginationParameters{ PageId = 1, PageSize = 10 };
            var orderItems = new List<OrderItem>
            {
                MockData.CreateMockOrderItemWithSpecificOrder(orderId),
                MockData.CreateMockOrderItemWithSpecificOrder(orderId)
            };

            _mockRepo.Setup(r => r.getAllOrderItemsByOrderIdAsync(orderId, pagination)).ReturnsAsync(orderItems);

            // Act
            var result = await _orderItemService.getAllOrderItemsByOrderIdAsync(orderId, pagination);

            // Assert
            result.Should().BeEquivalentTo(orderItems);
            result.All(oi => oi.OrderId == orderId).Should().BeTrue();
        }
    }
}