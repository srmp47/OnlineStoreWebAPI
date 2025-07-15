using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]
    public class ProductMutation
    {
        //TODO: check validation of Data Annotations

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Product> DeleteProduct(int id, [Service] IProductRepository productRepository)
        {
            var product = await productRepository.getProductByIdAsync(id);
            if (product == null)
            {
                throw new GraphQLException($"Product with ID {id} not found.");
            }

            var deletedProduct = await productRepository.deleteProductByIdAsync(id);
            if (deletedProduct == null)
            {
                throw new GraphQLException("Failed to delete product.");
            }

            return deletedProduct;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Product> CreateProduct
            (ProductDTO input, [Service] IProductRepository productRepository,
            [Service] AutoMapper.IMapper mapper)
        {
            if (input == null) throw new GraphQLException("enter a product");
            var product = mapper.Map<Product>(input);
            return await productRepository.createNewProductAsync(product);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Product> UpdateProduct
            (int id, ProductUpdateDTO input, [Service] IProductRepository productRepository,
            [Service] AutoMapper.IMapper mapper)
        {
            if (input == null) throw new GraphQLException("enter a product");
            var isValidId = await productRepository.isThereProductWithIdAsync(id);
            if (!isValidId) throw new GraphQLException("invalid product id");
            var product = await productRepository.getProductByIdAsync(id);
            return await productRepository.updateProductAsync(id, input);
        }
    }
}
