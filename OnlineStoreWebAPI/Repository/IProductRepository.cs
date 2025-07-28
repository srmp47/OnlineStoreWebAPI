using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IProductRepository
    {
        public Task<Product?> getProductByIdAsync(int productId);
        public Task<IEnumerable<Product>> getAllProductsAsync(PaginationParameters paginationParameters, string? search);
        public  Task createNewProductAsync(Product product);
        public Task<Product> updateProductAsync(Product product);
        public Task<bool> deleteProductByIdAsync(int id);
        public Task<bool> isThereProductWithIdAsync(int id);
        public Task<int> getStockQuantityOfProduct(int id);
    }
}
