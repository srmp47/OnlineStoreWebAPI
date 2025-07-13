using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
   [ExtendObjectType(typeof(UserQuery))]
    public class ProductQuery
    {
        public async Task<IEnumerable<Product>> GetProducts([Service] IProductRepository productRepository)
        {
            return await productRepository.getAllProductsAsync
                (new Pagination.PaginationParameters { PageId = 1, PageSize = 100 },null);
        }

        public async Task<Product?> GetProductById(int id, [Service] IProductRepository productRepository)
        {
            return await productRepository.getProductByIdAsync(id);
        }

        public async Task<bool> IsProductExists(int id, [Service] IProductRepository productRepository)
        {
            return await productRepository.isThereProductWithIdAsync(id);
        }
    }
}
