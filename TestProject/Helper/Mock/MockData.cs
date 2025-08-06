using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Enum;
using AutoFixture;
using OnlineStoreWebAPI.Repository;

namespace TestProject.Helper.Mock
{
    public static class MockData
    {
        private static readonly Fixture Fixture = new Fixture();
        
        static MockData()
        {
            // Configure AutoFixture to handle circular references
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public static User CreateMockUser()
        {
            return new User
            {
                firstName = Fixture.Create<string>(),
                lastName = Fixture.Create<string>(),
                email = Fixture.Create<string>(),
                password = Fixture.Create<string>(),
                address = Fixture.Create<string>(),
                isActive = true,
                role = "Customer"
            };
        }

        public static Product CreateMockProduct()
        {
            return new Product
            {
                name = Fixture.Create<string>(),
                description = Fixture.Create<string>(),
                price = Fixture.Create<double>(),
                StockQuantity = Fixture.Create<int>()
            };
        }

        public static Product CreateMockProductWithSpecificPrice(int price)
        {
            return new Product
            {
                name = Fixture.Create<string>(),
                description = Fixture.Create<string>(),
                price = price,
                StockQuantity = Fixture.Create<int>()
            };
        }

        public static Order CreateMockOrder()
        {
            return new Order
            {
                
                date = DateTime.UtcNow,
                totalAmount = Fixture.Create<double>(),
                status = OrderStatus.Pending
            };
        }

        public static Order CreateMockCompleteOrderWithOrderItems()
        {
            var order = Fixture.Create<Order>();
            return order;
        }

       

        public static Order CreateMockOrderForUser(int userId)
        {
            return new Order
            {
                userId = userId,
                date = DateTime.UtcNow,
                totalAmount = Fixture.Create<double>(),
                status = OrderStatus.Pending
            };
        }

        public static OrderItem CreateMockOrderItem(Order order,Product product)
        {
            int quantity = Fixture.Create<int>();
            return new OrderItem
            {
                Order = order,
                OrderId = order.OrderId,
                Product = product,
                productId = product.productId,
                quantity = quantity
            };
        }

        

        public static OrderItem CreateOrderItemWithProductIdAndOrderId(int productId, int orderId)
        {
            return new OrderItem
            {
                OrderId = orderId,
                productId = productId,
                quantity = Fixture.Create<int>()
            };
        }

        public static OrderItem CreateMockOrderItemWithSpecificProductAndOrder(int productId, int orderId)
        {
            return new OrderItem
            {
                OrderId = orderId,
                productId = productId,
                quantity = Fixture.Create<int>()
            };
        }
        public static OrderItem CreateMockOrderItemWithSpecificOrder(int orderId)
        {
            return new OrderItem
            {
                OrderId = orderId,
                productId = Fixture.Create<int>(),
                quantity = Fixture.Create<int>()
            };
        }

        public static List<User> CreateMockUsers(int count = 5)
        {
            return Fixture.CreateMany<User>(count).ToList();
        }
       

        public static List<Product> CreateMockProducts(int count = 10)
        {
            return Fixture.CreateMany<Product>(count).ToList();
        }

        public static List<Order> CreateMockOrders(int count = 5)
        {
            return Fixture.CreateMany<Order>(count).ToList();
        }
        public static List<Order> CreateMockOrdersForUser(int userId, int count = 5)
        {
            return Fixture.CreateMany<Order>(count).Select(o => CreateMockOrderForUser(userId)).ToList();
        }

        public static List<OrderItem> CreateMockOrderItems(Order order, Product product ,int count = 10)
        {
            return Fixture.CreateMany<OrderItem>(count).Select(oi => CreateMockOrderItem(order,product)).ToList();
        }

        public static List<OrderItem> CreateMockOrderItemsWithSpecificOrder(int orderId, int count = 10)
        {
            return Fixture.CreateMany<OrderItem>(count).Select(oi => CreateMockOrderItemWithSpecificOrder(orderId)).ToList();
        }

        // DTOs
        public static UserUpdateDTO CreateMockUserDto()
        {
            return new UserUpdateDTO
            {
                firstName = Fixture.Create<string>(),
                lastName = Fixture.Create<string>(),
                email = Fixture.Create<string>(),
                password = Fixture.Create<string>(),
                address = Fixture.Create<string>()
            };
        }

        public static ProductDTO CreateMockProductDto()
        {
            return new ProductDTO
            {
                name = Fixture.Create<string>(),
                description = Fixture.Create<string>(),
                price = Fixture.Create<double>(),
                StockQuantity = Fixture.Create<int>()
            };
        }

        public static OrderDTO CreateMockOrderDto()
        {
            return new OrderDTO
            {
                orderItemDTOs = new List<OrderItemDTO>()
            };
        }

        public static List<Product> CreateProductsToTestSearching()
        {
            var products = new List<Product> { new Product {name = "asda" , description = "dsoai  sd a a sduaosud " },
                new Product {name = "test" , description = "daspdadoaida op a ai" },
                new Product {description = " sjasda test sda" , name = "sda" } };

            return products;
        }
    }
} 