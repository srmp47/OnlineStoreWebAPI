using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> getAllProductsAsync();
        public Task<Product?> getProductByIdAsync(int id);
        public Task<bool> isThereProductByIdAsync(int id);
        public Task<Product> createNewProductAsync(Product product);
        public Task<Product> deleteProductByIdAsync(int id);
        public Task<Product> updateProductAsync(Product product);
        

    }
}
