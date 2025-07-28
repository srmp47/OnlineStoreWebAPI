using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/Product")]
    [ApiController]
    // TODO when an order is completed , the price of it's prodcts should not be changed
    // user should see the price that is paid befor. not current price
    public class ProductController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProductService productService;
        public ProductController(IMapper mapper, IProductService productService)
        {
            this.mapper = mapper;
            this.productService = productService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> createNewProduct(ProductDTO inputProduct)
        {
            if (inputProduct == null) return BadRequest("input product is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            Product product = mapper.Map<Product>(inputProduct);
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var result = await productService.createNewProductAsync(product);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteProductById(int id)
        {
            var isValidId = await productService.isThereProductWithIdAsync(id);
            if (!isValidId) return NotFound("Product not exist");
            var result = await productService.deleteProductByIdAsync(id);
            return Ok(result);

        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> getAllProducts
            ([FromQuery] PaginationParameters paginationParameters, [FromQuery] string? search)
        {
            var result = await productService.getAllProductsAsync(paginationParameters, search);
            if (result == null) return NoContent();
            return Ok(result);

        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> getProductById(int id)
        {
            var product = await productService.getProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);

        }
        [HttpGet("{id}/isThere")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> isThereProductWithId(int id)
        {
            if (await productService.isThereProductWithIdAsync(id)) return Ok("There is");
            else return Ok("There is not");

        }

        [HttpPut("{id}/Update")]
        [Authorize(Roles = "Admin")]
        // TODO implement the changes of price
        public async Task<IActionResult> updateProduct(int id, [FromBody] ProductUpdateDTO productDTO)
        {
            if (productDTO == null) return BadRequest("Product is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var isValidId = await productService.isThereProductWithIdAsync(id);
            if (!isValidId) return BadRequest("Product Not Found");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var result = await productService.updateProductAsync(id, productDTO);
                return Ok(result);
            }
        }
        [HttpGet("{id}/stockQuantity of product")]
        [Authorize]
        public async Task<IActionResult> getStockQuantityOfProduct(int id)
        {
            var product = await productService.getProductByIdAsync(id);
            if (product == null) return NotFound("Product not found");
            return Ok(product.StockQuantity);

        }
        //[Authorize(Roles = "Admin")]
        //[HttpPatch("{id}")]
        //// implement changes of price
        //public async Task<IActionResult> PatchProductById(int id, [FromBody] JsonPatchDocument<ProductUpdateDTO> patchDoc)
        //{
        //    if (patchDoc == null)
        //        return BadRequest("Patch document is null");

        //    var product = await productService.getProductByIdAsync(id);
        //    if (product == null)
        //        return NotFound("Product not found");

        //    var productToPatch = mapper.Map<ProductUpdateDTO>(product);

        //    patchDoc.ApplyTo(productToPatch);

        //    if (!TryValidateModel(productToPatch))
        //        return BadRequest(ModelState);

        //    mapper.Map(productToPatch, product);
            //TODO
            //await productService.partialUpdateProduct(product);

        //    return NoContent();
        //}
    }
}
