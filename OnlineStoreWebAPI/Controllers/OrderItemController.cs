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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Product>>> getAllOrderItems
            ([FromQuery]PaginationParameters paginationParameters)
        {
            var result = await orderItemRepository.getAllOrderItemsAsync(paginationParameters);
            if (result == null) return NoContent();
            return Ok(result);

        }
        // TODO user should can add a few order items
        [Authorize]
        [HttpPost("AddOrderItem/{orderId}")]
        public async Task<IActionResult> createNewOrderItem(int orderId , OrderItemDTO orderItemDTO)
        {
            if (orderItemDTO == null) return BadRequest("input order item is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var isValidOrderId = await orderRepository.isThereOrderByIdAsync(orderId);
            if (!isValidOrderId) return BadRequest("Order not exist");
            var isValidProductId = await productRepository.
                isThereProductWithIdAsync(orderItemDTO.productId);
            if (!isValidProductId) return BadRequest("Product not exist");
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var order = await orderRepository.getOrderByOrderIdAsync(orderId);
            if (order == null || order.userId != currentUserId)
                return BadRequest("You can not add item to this order");
            var  quantity = await productRepository.getQuantityOfProduct(orderItemDTO.productId);
            if (order.status == OrderStatus.Pending && orderItemDTO.quantity > quantity )
                return BadRequest("There is not enough stock for this product");
            if(order.status == OrderStatus.Cancelled)
                return BadRequest("You can not add item to this order, this order is cancelled");
            if (order.status != OrderStatus.Cancelled && order.status != OrderStatus.Pending)
                return BadRequest("The order is being prepared for shipment. you can not add new product. please place a new order");
            if (orderItemDTO.quantity <= 0)
                return BadRequest("You can not add order item with zero or negative quantity");
            OrderItem orderItem = mapper.Map<OrderItem>(orderItemDTO);
            orderItem.OrderId = orderId;
            await orderItemRepository.setOrderAndProductInOrderItem(orderItem);
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var result = await orderItemRepository.createNewOrderItemAsync(orderItem);
            return Ok(result);
        }
        // user should can only delete his/her order items.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteOrderItemById(int id)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(id);
            if (orderItem == null || orderItem.Order.userId != currentUserId)  
                return BadRequest("You can not delete this order item");
            var result = await orderItemRepository.deleteOrderItemByIdAsync(id);
            return Ok(result);

        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> getOrderItemById(int id)
        {
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(id);
            if (orderItem == null) return NotFound();
            return Ok(orderItem);

        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereOrderItemWithId(int id)
        {
            if (await orderItemRepository.isThereOrderItemById(id)) return Ok("There is");
            else return Ok("There is not");

        }
        [HttpPatch("{id}/changeQuantity/{quantity}")]
        [Authorize]
        public async Task<IActionResult> changeQuantityByOrderItemId(int id,int quantity)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(id);
            if (orderItem == null || orderItem.Order.userId != currentUserId)
            {
                return BadRequest("you don't have this order item");
            }
            if (orderItem.Order.status == OrderStatus.Cancelled)
                return Conflict("You can not change this order item, this order is cancelled");
            if (quantity < 0 || quantity > orderItem.Product.StockQuantity + orderItem.quantity)
                return BadRequest("there is not this number of products");
            var result = await orderItemRepository.changeQuantityByOrderItemId(id, quantity);
            return Ok(orderItem);

        }
    }
}
