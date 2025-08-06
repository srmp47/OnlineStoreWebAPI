using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Enum;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Integration
{
    public static class TestDataSeeder
    {
        public static void SeedTestData(OnlineStoreDBContext context)
        {
            // Clear existing data
            context.Users.RemoveRange(context.Users);
            context.Products.RemoveRange(context.Products);
            context.Orders.RemoveRange(context.Orders);
            context.OrderItems.RemoveRange(context.OrderItems);
            context.SaveChanges();

            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    userId = 1,
                    firstName = "John",
                    lastName = "Doe",
                    email = "john.doe@example.com",
                    password = "hashedpassword123",
                    address = "123 Main St, City, State",
                    isActive = true,
                    role = "Customer"
                },
                new User
                {
                    userId = 2,
                    firstName = "Jane",
                    lastName = "Smith",
                    email = "jane.smith@example.com",
                    password = "hashedpassword456",
                    address = "456 Oak Ave, City, State",
                    isActive = true,
                    role = "Admin"
                },
                new User
                {
                    userId = 3,
                    firstName = "Alice",
                    lastName = "Johnson",
                    email = "AliceJohnson@gmal.com",
                    password = "hashedpassword789",
                    address = "789 Pine Rd, City, State",
                    isActive = false,
                    role = "Customer"
                }
            };

            context.Users.AddRange(users);

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    productId = 1,
                    name = "Laptop",
                    description = "High-performance laptop",
                    price = 999.99,
                    StockQuantity = 10
                },
                new Product
                {
                    productId = 2,
                    name = "Smartphone",
                    description = "Latest smartphone model",
                    price = 699.99,
                    StockQuantity = 15
                },
                new Product
                {
                    productId = 3,
                    name = "Headphones",
                    description = "Wireless noise-canceling headphones",
                    price = 199.99,
                    StockQuantity = 25
                }
            };

            context.Products.AddRange(products);

            // Seed Orders
            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    userId = 1,
                    date = DateTime.Now.AddDays(-5),
                    totalAmount = 1199.98,
                    status = OrderStatus.Pending
                },
                new Order
                {
                    OrderId = 2,
                    userId = 1,
                    date = DateTime.Now.AddDays(-2),
                    totalAmount = 199.99,
                    status = OrderStatus.Completed
                },
                new Order
                {
                    OrderId = 3,
                    userId = 2,
                    date = DateTime.Now.AddDays(-1),
                    totalAmount = 699.99,
                    status = OrderStatus.Cancelled

                }
            };

            context.Orders.AddRange(orders);

            // Seed OrderItems
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 1,
                    OrderId = 1,
                    productId = 1,
                    quantity = 1,
                    price = 999.99
                },
                new OrderItem
                {
                    OrderItemId = 2,
                    OrderId = 1,
                    productId = 3,
                    quantity = 1,
                    price = 199.99
                },
                new OrderItem
                {
                    OrderItemId = 3,
                    OrderId = 2,
                    productId = 3,
                    quantity = 1,
                    price = 199.99
                }
            };

            context.OrderItems.AddRange(orderItems);

            context.SaveChanges();
        }

        public static void ClearTestData(OnlineStoreDBContext context)
        {
            context.OrderItems.RemoveRange(context.OrderItems);
            context.Orders.RemoveRange(context.Orders);
            context.Products.RemoveRange(context.Products);
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }

        
    
}
} 