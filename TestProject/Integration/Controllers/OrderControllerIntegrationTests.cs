using FluentAssertions;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Enum;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TestProject.Integration.Controllers
{
    public class OrderControllerIntegrationTests : BaseIntegrationTest
    {
        public OrderControllerIntegrationTests(TestWebApplicationFactory<Program> factory) : base(factory)
        {
        }
        [Fact]
        public async Task GetAllOrdersOfCurrentUser_ShouldReturnAdminOrders()
        {
            // Act
            var response = await GetAsync("/api/Order/show my orders?pageid=1&pagesize=10");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<Order>>(content);
            orders.Should().NotBeNull();
            orders.Should().HaveCount(1);
            orders.Should().OnlyContain(o => o.userId == 2);

        }
        [Fact]
        public async Task GetAllOrdersOfCurrentUser_CurrentUserHasNoOrder_ShouldReturnNoContent()
        {
            // Arrange
            ClearOrderOfUser(2);
            // Act
            var response = await GetAsync("/api/Order/show my orders?pageid=1&pagesize=10");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            ResetDatabase();
        }
        [Fact]
        public async Task GetAllOrders_ShouldReturnAllOrders()
        {
            // Act
            var response = await GetAsync("/api/Order?pageid=1&pagesize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<Order>>(content);
            orders.Should().NotBeNull();
            orders.Should().HaveCount(3); // Based on seeded data
        }
        
        [Fact]
        public async Task getAllOrders_WithoutAnyOrder_ShouldReturnNoContent()
        {
            // Arrange
            ClearDatabase();

            // Act
            var response = await GetAsync("/api/Order?pageid=1&pagesize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            ResetDatabase();
        }
        [Fact]
        public async Task IsThereOrderWithId_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            int orderId = 999;
            // Act
            var response = await GetAsync($"/api/Order/{orderId}/isThere");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("There is not");
        }

        [Fact]
        public async Task GetOrderById_WithValidId_ShouldReturnOrder()
        {
            // Arrange
            int orderId = 1;

            // Act
            var response = await GetAsync($"/api/Order/{orderId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(content);
            order.Should().NotBeNull();
            order.OrderId.Should().Be(orderId);
            order.userId.Should().Be(1);
            order.status.Should().Be(OrderStatus.Pending);
        }

        [Fact]
        public async Task GetOrderById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            int orderId = 999;

            // Act
            var response = await GetAsync($"/api/Order/{orderId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateOrder_WithValidData_ShouldReturnCreatedOrder()
        {
            // Arrange
            var newOrder = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>
                {
                    new OrderItemDTO
                    {
                        productId = 2, 
                        quantity = 1
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/Order/AddOrder", newOrder);
         

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsStringAsync();
            var createdOrder = JsonConvert.DeserializeObject<Order>(content);
            createdOrder.Should().NotBeNull();
            createdOrder.userId.Should().Be(2); // User ID from the request context
            createdOrder.totalAmount.Should().BeGreaterThan(0);
            createdOrder.status.Should().Be(OrderStatus.Pending);
            createdOrder.date.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task CreaeOrder_WithNullInput_ShouldReturnBadRequest()
        {
            // Arrange
            OrderDTO orderDTO = null;

            // Act
            var response = await PostAsync("/api/Order/AddOrder", orderDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await  response.Content.ReadAsStringAsync();
            content.Should().Contain("The inputOrder field is required.");

        }
        [Fact]
        public async Task CreateOrder_WithNullOrderItems_ShouldReturnBadRequest()
        {
            // Arrange
            OrderDTO orderDTO = new OrderDTO();

            // Act
            var response = await PostAsync("/api/Order/AddOrder", orderDTO);

            // Asseret
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("you must have at least one order item in your order");
        }

        [Fact]
        public async Task CreateOrder_WithZeroOrderItems_ShouldReturnBadRequest()
        {
            // Arrange
            OrderDTO orderDTO = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>()
            };

            // Act
            var response = await PostAsync("/api/Order/AddOrder", orderDTO);

            // Asseret
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("you must have at least one order item in your order");
        }

        [Fact]
        public async Task CreateOrder_WithInvalidQuantityOfProducts_ShouldReturnBadRequest()
        {
            // Arrange
            var orderDTO = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>
                {
                    new OrderItemDTO
                    {
                        productId = 1,
                        quantity = 1000
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/Order/AddOrder", orderDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync(); 
            content.Should().Be("There is not enough stock for product with id 1");
        }
        [Fact]
        public async Task CreateOrder_WithNegativeQuantityOfProduct_ShouldReturnBadRequest()
        {
            // Arrange
            var orderDTO = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>
                {
                    new OrderItemDTO
                    {
                        productId = 3,
                        quantity = -5
                    }
                }
            };
            // Act
            var response = await PostAsync("api/Order/AddOrder",orderDTO);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("You can not add order item with zero or negative quantity");

        }

        [Fact]
        public async Task CreateOrder_WithInvalidProductId_ShouldReturnBadRequest()
        {
            // Arrange
            var newOrder = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>
                {
                    new OrderItemDTO
                    {
                        productId = 999,
                        quantity = 1
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/Order/addOrder", newOrder);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be($"Product with id {newOrder.orderItemDTOs.ElementAt(0).productId} not exist");
        }
       

        [Fact]
        public async Task UpdateOrderStatus_WithValidData_ShouldReturnUpdatedOrder()
        {
            // Arrange
            int orderId = 1;
            var updateOrder = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>()
            };

            var newStatus = OrderStatus.Processing;
            var requestUrl = $"/api/Order/{orderId}/changeStatus/{newStatus}";


            // Act 
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl);
            var response = await _client.SendAsync(request);

            


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify order is updated
            response = await GetAsync($"/api/Order/{orderId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var updatedOrder = JsonConvert.DeserializeObject<Order>(content);
            updatedOrder.status.Should().Be(newStatus);
        }

        [Fact]
        public async Task UpdateOrderStatus_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            int orderId = 999;
            var updateOrder = new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>()
            };

            // Act
            var requestUrl = $"/api/Order/{orderId}/changeStatus/{OrderStatus.Processing}";
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl);
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteOrder_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            int orderId = 3;

            // Act
            var response = await DeleteAsync($"/api/Order/{orderId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify order is deleted
            var getResponse = await GetAsync($"/api/Order/{orderId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            ResetDatabase();
        }

        [Fact]
        public async Task DeleteOrder_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            int orderId = 999;


            // TODO changing the default role of user
            //_client.DefaultRequestHeaders.Remove("Authorization");
            //_client.DefaultRequestHeaders.Add(
            //    "Authorization",
            //    TestAuthHelper.GetTestAuthorizationHeader(role: "Admin", userId: "1")
            //);


            // Act
            var response = await DeleteAsync($"/api/Order/{orderId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetOrdersByUserId_WithValidUserId_ShouldReturnUserOrders()
        {
            // Arrange
            int userId = 1;

            // Act
            var response = await GetAsync($"/api/Order/Orders of user/{userId}?pageid=1&pagesize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<Order>>(content);
            orders.Should().NotBeNull();
            orders.Should().HaveCount(2); 
            orders.Should().OnlyContain(o => o.userId == userId);
        }

        [Fact]
        public async Task GetOrdersByUserId_WithInvalidUserId_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = 999;

            // Act
            var response = await GetAsync($"/api/Order/Orders of User/{userId}?pageid=1&pagesize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("user not found");
        }
        [Fact]
        public async Task getOrdersByUserId_UserWithNoOrder_ShuldReturnNoContent()
        {
            // Arrange
            var userId = 3; // User with no orders
            // Act
            var response = await GetAsync($"/api/Order/Orders of User/{userId}?pageid=1&pagesize=10");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task IsThereOrderWithId_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            int orderId = 1;
            // Act
            var response = await GetAsync($"/api/Order/{orderId}/isThere");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("There is");
        }
      



    }
} 