using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly OnlineStoreDBContext context;
        public ProductRepository(OnlineStoreDBContext inputContext)
        {
            this.context = inputContext;
        }

        public Task<Product> createNewProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<Product> deleteProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> getAllProductsAsync()
        {
            return await context.Products.OrderBy(p => p.productId).ToListAsync();
        }

        public async Task<Product?> getProductByIdAsync(int id)
        {
            return await context.Products.OrderBy(p => p.productId).Where(p => p.productId == id).FirstOrDefaultAsync();
        }

        public async Task<bool> isThereProductById(int id)
        {
            return await context.Products.AnyAsync(p => p.productId == id);
        }

        public Task<bool> isThereProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> updateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
