using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Helper;
using TestProject.Helper.Mock;

namespace TestProject.ServiceTest
{
    public class OrderServiceTests:ServiceTestBase
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepo = CreateMock<IOrderRepository>();
            _orderService = new OrderService(_mockRepo.Object, Mapper);
        }
        [Fact]
        public async Task CanCelOrderByIdAsync_ShouldCancelOrder()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            _mockRepo.Setup(r => r.getOrderByIdAsync(order.OrderId)).ReturnsAsync(order);

            // Act
            await _orderService.cancelOrderByIdAsync(order.OrderId);

            // Assert
            _mockRepo.Verify(r => r.getOrderByIdAsync(order.OrderId), Times.Once());
            order.status.Should().Be(OrderStatus.Cancelled);
        }
        [Fact]
        public async Task CreateNewOrderAsync_ShouldCreateNewOrder()
        {
            // Arrange
            var order = new Order
            {
                totalAmount = 0,
                orderItems = new List<OrderItem>
                {
                    new OrderItem { price = 50 },
                    new OrderItem { price = 30 }
                }
            };
            _mockRepo.Setup(r => r.createNewOrderAsync(order)).Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.createNewOrderAsync(order);

            // Assert
            _mockRepo.Verify(r => r.createNewOrderAsync(order), Times.Once);
            result.totalAmount.Should().Be(80);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrders()
        {
            // Arrange
            var pagination = new PaginationParameters { PageId = 1 , PageSize = 10 };
            var orders = MockData.CreateMockOrders() ;
            _mockRepo.Setup(r => r.getAllOrdersAsync(pagination)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.getAllOrdersAsync(pagination);

            // Assert
            result.Should().BeEquivalentTo(orders);
            _mockRepo.Verify(r => r.getAllOrdersAsync(pagination), Times.Once());
        }

        [Fact]
        public async Task GetAllOrdersOfUserByIdAsync_ShouldReturnUserOrders()
        {
            // Arrange
            int userId = 1;
            var pagination = new PaginationParameters { PageId = 1 , PageSize =10};
            var orders = MockData.CreateMockOrders();
            _mockRepo.Setup(r => r.getAllOrdersOfUserByIdAsync(userId, pagination))
                     .ReturnsAsync(orders);

            // Act
            var result = await _orderService.getAllOrdersOfUserByIdAsync(userId, pagination);

            // Assert
            result.Should().BeEquivalentTo(orders);
            _mockRepo.Verify(r => r.getAllOrdersOfUserByIdAsync(userId, pagination), Times.Once());
        }

        [Fact]
        public async Task GetOrderByOrderIdAsync_ShouldReturnOrder()
        {
            // Arrange
            int orderId = 1;
            var order = MockData.CreateMockOrder();
            _mockRepo.Setup(r => r.getOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _orderService.getOrderByOrderIdAsync(orderId);

            // Assert
            result.Should().Be(order);
            _mockRepo.Verify(r => r.getOrderByIdAsync(orderId), Times.Once());
        }

        [Fact]
        public async Task GetOrderByOrderIdAsync_WithInValidId_ReturnNull()
        {
            // Arrange
            int orderId = 999;
            _mockRepo.Setup(r => r.getOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.getOrderByOrderIdAsync(orderId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task IsThereOrderByIdAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            int orderId = 1;
            _mockRepo.Setup(r => r.isThereOrderWithIdAsync(orderId)).ReturnsAsync(true);

            // Act
            var result = await _orderService.isThereOrderByIdAsync(orderId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldCallRepository()
        {
            // Arrange
            var order = MockData.CreateMockOrder();
            _mockRepo.Setup(r => r.updateOrder(order)).Returns(Task.CompletedTask);

            // Act
            await _orderService.updateOrderAsync(order);

            // Assert
            _mockRepo.Verify(r => r.updateOrder(order), Times.Once());
        }

        [Fact]
        public async Task DeleteOrderByIdAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            int orderId = 1;
            _mockRepo.Setup(r => r.deleteOrderWithIdAsync(orderId)).ReturnsAsync(true);

            // Act
            var result = await _orderService.deleteOrderByIdAsync(orderId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SetUserInOrder_ShouldSetUserIdAndUser()
        {
            // Arrange
            int userId = 1;
            var order = MockData.CreateMockOrderForUser(userId);
            var user = MockData.CreateMockUser();
            user.userId = userId;
            _mockRepo.Setup(r => r.setUserInOrder(order, userId))
                     .Callback<Order, int>((o, id) => o.User = user);

            // Act
            await _orderService.setUserInOrder(order, userId);

            // Assert
            order.userId.Should().Be(userId);
            order.User.Should().Be(user);
            _mockRepo.Verify(r => r.setUserInOrder(order, userId), Times.Once());
        }

        [Fact]
        public async Task SetOrderAndProductInOrderItem_ShouldSetProperties()
        {
            // Arrange
            var orderItem = new OrderItem { OrderId = 1, productId = 1 };
            var order = new Order { OrderId = orderItem.OrderId };
            var product = new Product { productId = orderItem.productId };

            _mockRepo.Setup(r => r.setOrderAndProductInOrderItem(orderItem))
                     .Callback<OrderItem>(item =>
                     {
                         item.Order = order;
                         item.Product = product;
                     });

            // Act
            await _orderService.setOrderAndProductInOrderItem(orderItem);

            // Assert
            orderItem.Order.Should().Be(order);
            orderItem.Product.Should().Be(product);
            _mockRepo.Verify(r => r.setOrderAndProductInOrderItem(orderItem), Times.Once());
        }

        [Fact]
        public async Task ChangeOrderStatusByOrderIdAsync_ShouldUpdateStatus()
        {
            // Arrange
            int orderId = 1;
            var newStatus = OrderStatus.Delivered;
            var order = MockData.CreateMockOrder();
            order.OrderId = orderId;
            order.status = OrderStatus.Pending;
            _mockRepo.Setup(r => r.getOrderByIdAsync(orderId)).ReturnsAsync(order);
            _mockRepo.Setup(r => r.updateOrder(order)).Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.changeOrderStatusByOrderIdAsync(orderId, newStatus);

            // Assert
            result.status.Should().Be(newStatus);
            _mockRepo.Verify(r => r.updateOrder(order), Times.Once());
        }

        [Fact]
        public async Task GetUserIdOfOrder_ShouldReturnUserId()
        {
            // Arrange
            int orderId = 1;
            int expectedUserId = 5;
            _mockRepo.Setup(r => r.getUserIdOfOrder(orderId)).ReturnsAsync(expectedUserId);

            // Act
            var result = await _orderService.getUserIdOfOrder(orderId);

            // Assert
            result.Should().Be(expectedUserId);
        }

        [Fact]
        public void SetPricesOfOrderItems_ShouldCalculateCorrectPrices()
        {
            // Arrange
            var order = new Order
            {
                orderItems = new List<OrderItem>
                {
                    new OrderItem { quantity = 2, Product = new Product { price = 10 } },
                    new OrderItem { quantity = 3, Product = new Product { price = 20 } }
                }
            };

            // Act
            _orderService.setPricesOfOrderItems(order);

            // Assert
            order.orderItems.ElementAt(0).price.Should().Be(20);  // 2 * 10
            order.orderItems.ElementAt(1).price.Should().Be(60);  // 3 * 20
        }



    }
}
