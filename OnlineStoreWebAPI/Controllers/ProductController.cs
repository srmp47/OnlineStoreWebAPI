using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController: ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        public ProductController(IMapper mapper, IProductRepository productRepository)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> createNewProduct(ProductDTO inputProduct)
        {
            if (inputProduct == null) return BadRequest("input product is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            Product product = mapper.Map<Product>(inputProduct);
            if(!ModelState.IsValid) return BadRequest("Bad Request");
            var result = await productRepository.createNewProductAsync(product);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteProductById(int id)
        {
            var isValidId = await productRepository.isThereProductWithIdAsync(id);
            if (!isValidId) return NotFound("Product not exist");
            var result = await productRepository.deleteProductByIdAsync(id);
            return Ok(result);

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> getAllProducts()
        {
            var result = await productRepository.getAllProductsAsync();
            if (result == null) return NoContent();
            return Ok(result);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getProductById(int id)
        {
            var product = await productRepository.getProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
            
        }
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereProductWithId(int id)
        {
            if (await productRepository.isThereProductWithIdAsync(id)) return Ok("There is");
            else return Ok("There is not");

        }
        // Update is not fully implemented 
        [HttpPost("{id}/Update")]
        public async Task<IActionResult> updateProduct(int id, [FromBody] ProductDTO inputProduct)
        {
            if (inputProduct == null) return BadRequest("Product is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var isValidId = await productRepository.isThereProductWithIdAsync(id);
            if (!isValidId) return BadRequest("Product Not Found");
            var product = mapper.Map<Product>(inputProduct);
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var result = await productRepository.updateProductAsync(id,product);
                return Ok(result);
            }
        }

    }
}
