using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly OnlineStoreDBContext context;
        private readonly IMapper mapper;
        public ProductRepository(OnlineStoreDBContext inputContext, IMapper inputMapper)
        {
            this.context = inputContext;
            this.mapper = inputMapper;
        }

        public async Task<Product> createNewProductAsync(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> deleteProductByIdAsync(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.productId == id);
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> getAllProductsAsync()
        {
            return await context.Products.OrderBy(p => p.productId).ToListAsync();
        }

        public async Task<Product?> getProductByIdAsync(int id)
        {
            return await context.Products.OrderBy(p => p.productId).Where(p => p.productId == id).FirstOrDefaultAsync();
        }

       

        public async Task<bool> isThereProductWithIdAsync(int id)
        {
            return await context.Products.AnyAsync(p => p.productId == id);
        }

        public async Task<Product> updateProductAsync(int id,Product product)
        {
            var currentProduct = await context.Products.FirstOrDefaultAsync
                (p => p.productId == id);
            currentProduct.name = product.name;
            currentProduct.StockQuantity = product.StockQuantity;
            currentProduct.price = product.price;
            context.Update(currentProduct);
            await context.SaveChangesAsync();
            return product;
        }
    }
}
