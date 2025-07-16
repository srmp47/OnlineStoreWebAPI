using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

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

        public async Task<IEnumerable<Product>> getAllProductsAsync
            (PaginationParameters paginationParameters, string? search)
        {
            
            IQueryable<Product> products = context.Products;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                products = products.Where(p =>
                    p.name.ToLower().Contains(search) ||
                    (p.description != null && p.description.ToLower().Contains(search))
                );
            }

            products = products.
                Skip(paginationParameters.PageSize * (paginationParameters.PageId - 1))
                .Take(paginationParameters.PageSize);

            return await products.ToArrayAsync();
        }

        public async Task<Product?> getProductByIdAsync(int id)
        {
            return await context.Products.OrderBy(p => p.productId).Where(p => p.productId == id).FirstOrDefaultAsync();
        }

       

        public async Task<bool> isThereProductWithIdAsync(int id)
        {
            return await context.Products.AnyAsync(p => p.productId == id);
        }

       
        public async Task addToStockQuantity(int productId, int quantity)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.productId == productId);
            if (product != null)
            {
                product.StockQuantity += quantity;
                context.Update(product);
                await context.SaveChangesAsync();
            }
        }
        public async Task  removeFromStockQuantity(int productId, int quantity)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.productId == productId);
            if (product != null)
            {
                product.StockQuantity -= quantity;
                context.Update(product);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Product> updateProductAsync(int id,ProductUpdateDTO product)
        {
            var currentProduct = await context.Products.FirstOrDefaultAsync
                (p => p.productId == id);
            if (product.name != null) currentProduct.name = product.name;
            if (product.StockQuantity != null) currentProduct.StockQuantity =(int)product.StockQuantity;
            if (product.price != null) currentProduct.price = (double)product.price;
            if (product.description != null) currentProduct.description = product.description;
            context.Update(currentProduct);
            await context.SaveChangesAsync();
            return currentProduct;
        }

        public async Task setStockQuantity(int productId, int quantity)
        {
            var product = await  context.Products.FirstOrDefaultAsync(p => p.productId == productId);
            if (product != null)
            {
                product.StockQuantity = quantity;
                context.Update(product);
                await context.SaveChangesAsync();
            }
        }
    }
}
