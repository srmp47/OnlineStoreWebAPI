using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/OrderItem")]
    [ApiController]
    [Authorize]
    public class OrderItemController:ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrderItemRepository orderItemRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        public OrderItemController(IMapper mapper, IOrderItemRepository orderItemRepository
            ,IOrderRepository orderRepository , IProductRepository productRepository)
        {
            this.mapper = mapper;
            this.orderItemRepository = orderItemRepository;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
        }
        [HttpGet]
        //implement authentication :
        //[Authorize(Roles = "Admin,User")] , ....
        public async Task<ActionResult<IEnumerable<Product>>> getAllOrderItems
            ([FromQuery]PaginationParameters paginationParameters)
        {
            var result = await orderItemRepository.getAllOrderItemsAsync(paginationParameters);
            if (result == null) return NoContent();
            return Ok(result);

        }
        [HttpPost("AddOrderItem/{orderId}")]
        public async Task<IActionResult> createNewOrderItem(int orderId , OrderItemDTO orderItemDTO)
        {
            if (orderItemDTO == null) return BadRequest("input product is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var isValidOrderId = await orderRepository.isThereOrderByIdAsync(orderId);
            if (!isValidOrderId) return BadRequest("Order not exist");
            var isValidProductId = await productRepository.
                isThereProductWithIdAsync(orderItemDTO.productId);
            if (!isValidProductId) return BadRequest("Product not exist"); 
            OrderItem orderItem = mapper.Map<OrderItem>(orderItemDTO);
            orderItem.OrderId = orderId;
            await orderItemRepository.setOrderAndProductInOrderItem(orderItem);
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var result = await orderItemRepository.createNewOrderItemAsync(orderItem);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteOrderItemById(int id)
        {
            var isValidId = await orderItemRepository.isThereOrderItemById(id);
            if (!isValidId) return NotFound("Order item not exist");
            var result = await orderItemRepository.deleteOrderItemByIdAsync(id);
            return Ok(result);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getOrderItemById(int id)
        {
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(id);
            if (orderItem == null) return NotFound();
            return Ok(orderItem);

        }
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereOrderItemWithId(int id)
        {
            if (await orderItemRepository.isThereOrderItemById(id)) return Ok("There is");
            else return Ok("There is not");

        }
        [HttpPatch("{id}/changeQuantity/{quantity}")]
        public async Task<IActionResult> changeQuantityByOrderItemId(int id,int quantity)
        {
            var isValidId = await orderItemRepository.isThereOrderItemById(id);
            if (!isValidId) return NotFound("Order item not exist");
            if (quantity < 0) return BadRequest("invalid quantity!");
            var orderItem = await orderItemRepository.changeQuantityByOrderItemId(id, quantity);
            return Ok(orderItem);

        }
    }
}
