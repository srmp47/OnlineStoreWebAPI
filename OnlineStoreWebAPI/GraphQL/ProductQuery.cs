using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
   [ExtendObjectType(typeof(UserQuery))]
    public class ProductQuery
    {
        
        public async Task<IEnumerable<Product>> GetProducts([Service] IProductRepository productRepository,
            int pageId = 1 , int pageSize = 5)
        {
            return await productRepository.getAllProductsAsync
                (new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize },null);
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
