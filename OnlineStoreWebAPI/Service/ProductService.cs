using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository productRepository;
        private readonly IMapper mapper;
        public ProductService(ProductRepository productRepository, IMapper inputMapper)
        {
            this.productRepository = productRepository;
            this.mapper = inputMapper;
        }

        public async Task<Product> createNewProductAsync(Product product)
        {
            await productRepository.createNewProductAsync(product);
            return product;
        }

        public async Task<bool> deleteProductByIdAsync(int id)
        {
            var isDeletedSuccessfully = await productRepository.deleteProductByIdAsync(id);
            return isDeletedSuccessfully;
        }

        public async Task<IEnumerable<Product>> getAllProductsAsync
            (PaginationParameters paginationParameters, string? search)
        {
            
            var products = await productRepository.getAllProductsAsync(paginationParameters, search);
            return products;
        }

        public async Task<Product?> getProductByIdAsync(int id)
        {
            var prodoct = await productRepository.getProductByIdAsync(id);
            return prodoct;
        }

       

        public async Task<bool> isThereProductWithIdAsync(int id)
        {
            var isThere = await productRepository.isThereProductWithIdAsync(id);
            return isThere;
        }

       // TODO can you make it with more perforamance? (e.g just change the specific field of product [is quantity here])
        public async Task addToStockQuantity(int productId, int quantity)
        {
            var product = await productRepository.getProductByIdAsync(productId);
            product.StockQuantity += quantity;
            await productRepository.updateProductAsync(product);
        }
        public async Task  removeFromStockQuantity(int productId, int quantity)
        {
            var product = await productRepository.getProductByIdAsync(productId);
            product.StockQuantity -= quantity;
            await productRepository.updateProductAsync(product);
        }


        public async Task<Product> updateProductAsync(int id, ProductUpdateDTO productDTO)
        {
            var product = mapper.Map<Product>(productDTO);
            product.productId = id;
            var newProduct = await productRepository.updateProductAsync(product);
            return newProduct;
        }

        public async Task setStockQuantity(int productId, int quantity)
        {
            var product = await productRepository.getProductByIdAsync(productId);
            product.StockQuantity = quantity;
            await productRepository.updateProductAsync(product);
        }
        // TODO : see it :
        //public async Task partialUpdateProduct(Product product)
        //{
        //    await productRepository.updateProductAsync(product);
        //}

        public async Task<int> getQuantityOfProduct(int productId)
        {
            var quantity = await productRepository.getStockQuantityOfProduct(productId);
            return quantity;
        }
    }
}
