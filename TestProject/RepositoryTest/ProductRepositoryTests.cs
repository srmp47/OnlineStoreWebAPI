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

namespace TestProject.Repository
{
    public class ProductRepositoryTests : RepositoryTestBase
    {
        private readonly ProductRepository _productRepository;
        private readonly OnlineStoreDBContext _context;
        public ProductRepositoryTests()
        {
            this._context = GetDbContext();
            this._productRepository = new ProductRepository(_context);
        }
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            int numberOfProducts = 3;
            var products = MockData.CreateMockProducts(numberOfProducts);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            // Act
            var result = await _productRepository.getAllProductsAsync(new PaginationParameters { PageId = 1 , PageSize = products.Count},null);
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(numberOfProducts);
        }
        [Fact] async Task GetAllProductsAsync_WithSearch_ShouldReturnFilteredProducts()
        {
            // Arrange
            int numberOfProducts = 5;
            var products = MockData.CreateMockProducts(numberOfProducts);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            string searchTerm = products[2].name; // Assuming the third product's name is unique
            // Act
            var result = await _productRepository.getAllProductsAsync(new PaginationParameters { PageId = 1, PageSize = numberOfProducts }, searchTerm);
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().name.Should().Be(searchTerm);
        }
        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var product = MockData.CreateMockProduct();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            // Act
            var result = await _productRepository.getProductByIdAsync(product.productId);
            // Assert
            result.Should().NotBeNull();
            result.productId.Should().Be(product.productId);
            result.name.Should().Be(product.name);
        }
        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var result = await _productRepository.getProductByIdAsync(invalidId);
            // Assert
            result.Should().BeNull();
        }
        [Fact]
        public async Task CreateNewProductAsync_ShouldAddProduct()
        {
            // Arrange
            var product = MockData.CreateMockProduct();
            // Act
            await _productRepository.createNewProductAsync(product);
            // Assert
            var result = await _productRepository.getProductByIdAsync(product.productId);
            result.Should().NotBeNull();
            // TODO is checking only one field (here is name) is enogh?
            result.name.Should().Be(product.name);
        }
        [Fact]
        public async Task DeleteProductByIdAsync_WithValidId_ShouldDeleteProduct()
        {
            // Arrange
            var product = MockData.CreateMockProduct();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            // Act
            var isDeleted = await _productRepository.deleteProductByIdAsync(product.productId);
            // Assert
            isDeleted.Should().BeTrue();
            var result = await _productRepository.getProductByIdAsync(product.productId);
            result.Should().BeNull();
        }
        [Fact]
        public async Task DeleteProductByIdAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var isDeleted = await _productRepository.deleteProductByIdAsync(invalidId);
            // Assert
            isDeleted.Should().BeFalse();
        }
        [Fact]
        public async Task IsThereProductWithIdAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var product = MockData.CreateMockProduct();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            // Act
            var isThere = await _productRepository.isThereProductWithIdAsync(product.productId);
            // Assert
            isThere.Should().BeTrue();
        }
        [Fact]
        public async Task IsThereProductWithIdAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var isThere = await _productRepository.isThereProductWithIdAsync(invalidId);
            // Assert
            isThere.Should().BeFalse();
        }
        [Fact]
        public async Task GetStockQuantityOfProduct_WithValidId_ShouldReturnCorrectStockQuantity()
        {
            int quantity = 10;
            // Arrange
            var product = MockData.CreateMockProduct();
            product.StockQuantity = quantity; // Set stock quantity
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            // Act
            var stockQuantity = await _productRepository.getStockQuantityOfProduct(product.productId);
            // Assert
            stockQuantity.Should().Be(quantity);
        }
        [Fact]
        public async Task GetStockQuantityOfProduct_WithInvalidId_ShouldReturnZero()
        {
            // Arrange
            var invalidId = 999;
            // Act
            var stockQuantity = await _productRepository.getStockQuantityOfProduct(invalidId);
            // Assert
            stockQuantity.Should().Be(0);
        }
        [Fact]
        public async Task updateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            var product = MockData.CreateMockProduct();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            product.name = "Updated Product Name"; 
            // Act
            await _productRepository.updateProductAsync(product);
            // Assert
            var updatedProduct = await _productRepository.getProductByIdAsync(product.productId);
            updatedProduct.Should().NotBeNull();
            updatedProduct.name.Should().Be("Updated Product Name");
        }
        

    }
}
