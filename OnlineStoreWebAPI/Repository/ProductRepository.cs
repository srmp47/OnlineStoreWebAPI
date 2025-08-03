using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly OnlineStoreDBContext _context;
        public ProductRepository(OnlineStoreDBContext context)
        {
            _context = context;
        }
        public async Task createNewProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> deleteProductByIdAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.productId == id);
            if (product == null)
            {
                return false; // Product not found
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> getAllProductsAsync(PaginationParameters paginationParameters, string? search)
        {
            IQueryable<Product> products = _context.Products;

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

        public async Task<Product?> getProductByIdAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.productId == productId);
            return product;
        }

        public async Task<int> getStockQuantityOfProduct(int id)
        {
            var quantity = await  _context.Products
                .Where(p => p.productId == id)
                .Select(p => p.StockQuantity)
                .FirstOrDefaultAsync();
            return quantity;
        }

        public async Task<bool> isThereProductWithIdAsync(int id)
        {
            var isThere = await _context.Products.AnyAsync(p => p.productId == id);
            return isThere;
        }

        public async Task<Product> updateProductAsync(Product product)
        {
            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

       
    }
}
