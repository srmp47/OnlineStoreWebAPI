using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> getAllProductsAsync();
        public Task<Product?> getProductByIdAsync(int id);
        public Task<bool> isThereProductById(int id);
    }
}
