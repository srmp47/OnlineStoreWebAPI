using Xunit;
using FluentAssertions;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject
{
    public class BasicTests
    {
        [Fact]
        public void BasicTest_ShouldPass()
        {
            // Arrange
            var expected = 2;
            var actual = 1 + 1;

            // Act & Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DatabaseContext_ShouldBeCreated()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDbContext<OnlineStoreDBContext>(options =>
                options.UseInMemoryDatabase("TestDB"));

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var context = serviceProvider.GetRequiredService<OnlineStoreDBContext>();

            // Assert
            context.Should().NotBeNull();
            context.Users.Should().NotBeNull();
            context.Products.Should().NotBeNull();
            context.Orders.Should().NotBeNull();
            context.OrderItems.Should().NotBeNull();
        }

        [Fact]
        public void UserModel_ShouldHaveRequiredProperties()
        {
            // Arrange & Act
            var user = new User
            {
                userId = 1,
                firstName = "John",
                lastName = "Doe",
                email = "john@example.com",
                password = "password123",
                address = "123 Main St",
                isActive = true
            };

            // Assert
            user.Should().NotBeNull();
            user.userId.Should().Be(1);
            user.firstName.Should().Be("John");
            user.lastName.Should().Be("Doe");
            user.email.Should().Be("john@example.com");
        }

        [Fact]
        public void ProductModel_ShouldHaveRequiredProperties()
        {
            // Arrange & Act
            var product = new Product
            {
                productId = 1,
                name = "Test Product",
                description = "Test Description",
                price = 99.99,
                StockQuantity = 10
            };

            // Assert
            product.Should().NotBeNull();
            product.productId.Should().Be(1);
            product.name.Should().Be("Test Product");
            product.price.Should().Be(99.99);
            product.StockQuantity.Should().Be(10);
        }
    }
} 