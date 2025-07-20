using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/Product")]
    [ApiController]
    //TODO when user adds a product to  his/her order items , quantity of products should not be chnaged
    // quantity is changed when the order is completed...
    // TODO when an order is completed , the price of it's prodcts should not be changed
    // user should see the price that is paid befor. not currect price
    public class ProductController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        public ProductController(IMapper mapper, IProductRepository productRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> createNewProduct(ProductDTO inputProduct)
        {
            if (inputProduct == null) return BadRequest("input product is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            Product product = mapper.Map<Product>(inputProduct);
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var result = await productRepository.createNewProductAsync(product);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteProductById(int id)
        {
            var isValidId = await productRepository.isThereProductWithIdAsync(id);
            if (!isValidId) return NotFound("Product not exist");
            var result = await productRepository.deleteProductByIdAsync(id);
            return Ok(result);

        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> getAllProducts
            ([FromQuery] PaginationParameters paginationParameters, [FromQuery] string? search)
        {
            var result = await productRepository.getAllProductsAsync(paginationParameters, search);
            if (result == null) return NoContent();
            return Ok(result);

        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> getProductById(int id)
        {
            var product = await productRepository.getProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);

        }
        [HttpGet("{id}/isThere")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> isThereProductWithId(int id)
        {
            if (await productRepository.isThereProductWithIdAsync(id)) return Ok("There is");
            else return Ok("There is not");

        }

        [HttpPut("{id}/Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateProduct(int id, [FromBody] ProductUpdateDTO product)
        {
            if (product == null) return BadRequest("Product is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var isValidId = await productRepository.isThereProductWithIdAsync(id);
            if (!isValidId) return BadRequest("Product Not Found");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var result = await productRepository.updateProductAsync(id, product);
                return Ok(result);
            }
        }
        [HttpGet("{id}/stockQuantity of product")]
        [Authorize]
        public async Task<IActionResult> getStockQuantityOfProduct(int id)
        {
            var product = await productRepository.getProductByIdAsync(id);
            if (product == null) return NotFound("Product not found");
            return Ok(product.StockQuantity);

        }
    }
}
