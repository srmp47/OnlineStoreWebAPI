using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> getAllProductsAsync
            (PaginationParameters paginationParameters, string? search);
        public Task<Product?> getProductByIdAsync(int id);
        public Task<bool> isThereProductWithIdAsync(int id);
        public Task<Product> createNewProductAsync(Product product);
        public Task<bool> deleteProductByIdAsync(int id);
        public Task<Product> updateProductAsync(int id,ProductUpdateDTO product);
        public  Task addToStockQuantity(int productId, int quantity);
        public  Task removeFromStockQuantity(int productId, int quantity);
        public Task setStockQuantity(int productId , int quantity);
        // TODO
        //public Task partialUpdateProduct(Product product);
        public Task<int> getQuantityOfProduct(int productId);


    }
}
