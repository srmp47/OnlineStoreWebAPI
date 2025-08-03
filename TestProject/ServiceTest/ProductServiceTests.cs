using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineStoreWebAPI.DTO;
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
    public class ProductServiceTests : ServiceTestBase
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepo = CreateMock<IProductRepository>();
            _productService = new ProductService(_mockRepo.Object, Mapper);
        }

        [Fact]
        public async Task CreateNewProductAsync_ShouldCallRepository()
        {
            // Arrange
            var product = MockData.CreateMockProduct();
            _mockRepo.Setup(r => r.createNewProductAsync(product)).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.createNewProductAsync(product);

            // Assert
            result.Should().Be(product);
            _mockRepo.Verify(r => r.createNewProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteProductByIdAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            int productId = 1;
            _mockRepo.Setup(r => r.deleteProductByIdAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _productService.deleteProductByIdAsync(productId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts_WithoutSearch()
        {
            // Arrange
            var pagination = new PaginationParameters { PageId = 1 , PageSize = 10};
            var products = new List<Product> { MockData.CreateMockProduct() };
            _mockRepo.Setup(r => r.getAllProductsAsync(pagination, null)).ReturnsAsync(products);

            // Act
            var result = await _productService.getAllProductsAsync(pagination, null);

            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnFilteredProducts_WithSearch()
        {
            // Arrange
            var pagination = new PaginationParameters{ PageId = 1, PageSize = 10 };
            var searchTerm = "test";
            var products = MockData.CreateProductsToTestSearching();
            _mockRepo.Setup(r => r.getAllProductsAsync(pagination, searchTerm)).ReturnsAsync(products.Where(p =>
                    p.name.ToLower().Contains(searchTerm) ||
                    (p.description != null && p.description.ToLower().Contains(searchTerm))).ToList());

            var expectedProducts = new List<Product> { products.ElementAt(1), products.ElementAt(2) };
            // Act
            var result = await _productService.getAllProductsAsync(pagination, searchTerm);

            // Assert
            result.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            int productId = 1;
            var product = MockData.CreateMockProduct();
            _mockRepo.Setup(r => r.getProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _productService.getProductByIdAsync(productId);

            // Assert
            result.Should().Be(product);
        }

        [Fact]
        public async Task IsThereProductWithIdAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            int productId = 1;
            _mockRepo.Setup(r => r.isThereProductWithIdAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _productService.isThereProductWithIdAsync(productId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddToStockQuantity_ShouldIncreaseQuantity()
        {
            // Arrange
            int productId = 1;
            int initialQuantity = 10;
            int addQuantity = 5;

            var product = new Product
            {
                productId = productId,
                StockQuantity = initialQuantity
            };

            _mockRepo.Setup(r => r.getProductByIdAsync(productId)).ReturnsAsync(product);
            _mockRepo.Setup(r => r.updateProductAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

            // Act
            await _productService.addToStockQuantity(productId, addQuantity);

            // Assert
            product.StockQuantity.Should().Be(initialQuantity + addQuantity);
            _mockRepo.Verify(r => r.updateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task RemoveFromStockQuantity_ShouldDecreaseQuantity()
        {
            // Arrange
            int productId = 1;
            int initialQuantity = 10;
            int removeQuantity = 3;

            var product = new Product
            {
                productId = productId,
                StockQuantity = initialQuantity
            };

            _mockRepo.Setup(r => r.getProductByIdAsync(productId)).ReturnsAsync(product);
            _mockRepo.Setup(r => r.updateProductAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

            // Act
            await _productService.removeFromStockQuantity(productId, removeQuantity);

            // Assert
            product.StockQuantity.Should().Be(initialQuantity - removeQuantity);
            _mockRepo.Verify(r => r.updateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldMapAndUpdate()
        {
            // Arrange
            int productId = 1;
            var productDto = new ProductUpdateDTO
            {
                name = "Updated Product",
                description = "Updated Description",
                price = 99.99,
                StockQuantity = 50
            };

            var existingProduct = new Product
            {
                productId = productId,
                name = "Original",
                description = "Original",
                price = 10.00,
                StockQuantity = 10
            };

            _mockRepo.Setup(r => r.updateProductAsync(It.IsAny<Product>()))
                     .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.updateProductAsync(productId, productDto);

            // Assert
            result.productId.Should().Be(productId);
            result.name.Should().Be(productDto.name);
            result.description.Should().Be(productDto.description);
            result.price.Should().Be(productDto.price);
            result.StockQuantity.Should().Be(productDto.StockQuantity);
        }

        [Fact]
        public async Task SetStockQuantity_ShouldSetSpecificQuantity()
        {
            // Arrange
            int productId = 1;
            int newQuantity = 25;

            var product = new Product
            {
                productId = productId,
                StockQuantity = 10
            };

            _mockRepo.Setup(r => r.getProductByIdAsync(productId)).ReturnsAsync(product);
            _mockRepo.Setup(r => r.updateProductAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

            // Act
            await _productService.setStockQuantity(productId, newQuantity);

            // Assert
            product.StockQuantity.Should().Be(newQuantity);
            _mockRepo.Verify(r => r.updateProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task GetQuantityOfProduct_ShouldReturnStockQuantity()
        {
            // Arrange
            int productId = 1;
            int expectedQuantity = 50;
            _mockRepo.Setup(r => r.getStockQuantityOfProduct(productId)).ReturnsAsync(expectedQuantity);

            // Act
            var result = await _productService.getQuantityOfProduct(productId);

            // Assert
            result.Should().Be(expectedQuantity);
        }
    }
}