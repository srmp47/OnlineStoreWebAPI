using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]
    public class ProductMutation
    {
        //TODO: check validation of Data Annotations

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> DeleteProduct(int id, [Service] IProductService productService)
        {
            var product = await productService.getProductByIdAsync(id);
            if (product == null)
            {
                throw new GraphQLException($"Product with ID {id} not found.");
            }

            var deletedProduct = await productService.deleteProductByIdAsync(id);
            if (deletedProduct == null)
            {
                throw new GraphQLException("Failed to delete product.");
            }

            return deletedProduct;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Product> CreateProduct
            (ProductDTO input, [Service] IProductService productService,
            [Service] AutoMapper.IMapper mapper)
        {
            if (input == null) throw new GraphQLException("enter a product");
            var context = new ValidationContext(input);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(input, context, results, validateAllProperties: true))
            {
                var messages = results.Select(r => r.ErrorMessage);
                throw new GraphQLException(string.Join(" | ", messages));
            }
            var product = mapper.Map<Product>(input);
            return await productService.createNewProductAsync(product);
        }
        // check
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Product> UpdateProduct
            (int id, ProductUpdateDTO productDTO, [Service] IProductService productService,
            [Service] AutoMapper.IMapper mapper)
        {
            if (productDTO == null) throw new GraphQLException("enter a product");
            var isValidId = await productService.isThereProductWithIdAsync(id);
            if (!isValidId) throw new GraphQLException("invalid product id");
            var context = new ValidationContext(productDTO);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(productDTO, context, results, validateAllProperties: true))
            {
                var messages = results.Select(r => r.ErrorMessage);
                throw new GraphQLException(string.Join(" | ", messages));
            }
            return await productService.updateProductAsync(id, productDTO);
        }
    }
}
