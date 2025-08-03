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
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        public OrderItemController(IMapper mapper, IOrderItemService orderItemService
            ,IOrderService orderService , IProductService productService)
        {
            this.mapper = mapper;
            this.orderItemService = orderItemService;
            this.orderService = orderService;
            this.productService = productService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Product>>> getAllOrderItems
            ([FromQuery]PaginationParameters paginationParameters)
        {
            var result = await orderItemService.getAllOrderItemsAsync(paginationParameters);
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
            var isValidOrderId = await orderService.isThereOrderByIdAsync(orderId);
            if (!isValidOrderId) return BadRequest("Order not exist");
            var isValidProductId = await productService.
                isThereProductWithIdAsync(orderItemDTO.productId);
            if (!isValidProductId) return BadRequest("Product not exist");
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var order = await orderService.getOrderByOrderIdAsync(orderId);
            if (order == null || order.userId != currentUserId)
                return BadRequest("You can not add item to this order");
            var  quantity = await productService.getQuantityOfProduct(orderItemDTO.productId);
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
            //await orderItemService.setOrderAndProductInOrderItem(orderItem);
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            var result = await orderItemService.createNewOrderItemAsync(orderItem);
            return Ok(result);
        }
        // user should can only delete his/her order items.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteOrderItemById(int id)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var orderItem = await orderItemService.getOrderItemByOrderItemId(id);
            if(orderItem == null ) return BadRequest("Order item not exist");
            if (orderItem.Order.userId != currentUserId)  
                return BadRequest("You can not delete this order item. it's not for you");
            if (orderItem.Order.status == OrderStatus.Cancelled)
                return BadRequest("the order is cancelled");
            if (orderItem.Order.status != OrderStatus.Pending)
                return BadRequest("The order is being prepared for shipment. you can not delete this order item");
            var result = await orderItemService.deleteOrderItemByIdAsync(id);
            return Ok(result);

        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> getOrderItemById(int id)
        {
            var orderItem = await orderItemService.getOrderItemByOrderItemId(id);
            if (orderItem == null) return NotFound();
            return Ok(orderItem);

        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereOrderItemWithId(int id)
        {
            if (await orderItemService.isThereOrderItemById(id)) return Ok("There is");
            else return Ok("There is not");

        }
        [HttpPatch("{id}/changeQuantity/{quantity}")]
        [Authorize]
        public async Task<IActionResult> changeQuantityByOrderItemId(int id,int quantity)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var orderItem = await orderItemService.getOrderItemByOrderItemId(id);
            if (orderItem == null || orderItem.Order.userId != currentUserId)
            {
                return BadRequest("you don't have this order item");
            }
            if (orderItem.Order.status == OrderStatus.Cancelled)
                return Conflict("You can not change this order item, this order is cancelled");
            if(orderItem.Order.status != OrderStatus.Pending)
                return Conflict("You can not change this order item, this order is being prepared for shipment");
            if (quantity < 0 || quantity > orderItem.Product.StockQuantity + orderItem.quantity)
                return BadRequest("there is not this number of products");
            var result = await orderItemService.changeQuantityByOrderItemId(id, quantity);
            return Ok(orderItem);

        }
        [HttpGet("OrderItemsOfOrder/{orderId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> getAllOrderItemsByOrderId(int orderId, [FromQuery] PaginationParameters paginationParameters)
        {
            var isValidId = await orderService.isThereOrderByIdAsync(orderId);
            if (!isValidId) return BadRequest("Order not exist");
            var result = orderItemService.getAllOrderItemsByOrderIdAsync(orderId,paginationParameters);
            return Ok(result);
        }
        [HttpGet("OrderItemsOfMyOrder/{orderId}")]
        [Authorize]
        public async Task<IActionResult> getAllOrderItemsOfMyOrder(int orderId , [FromQuery] PaginationParameters paginationParameters)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var isValidId = await orderService.isThereOrderByIdAsync(orderId);
            if (!isValidId) return BadRequest("Order not exist");
            var order = await orderService.getOrderByOrderIdAsync(orderId);
            if (order == null || order.userId != currentUserId) return BadRequest("You can not see this order");
            var result = await orderItemService.getAllOrderItemsByOrderIdAsync(orderId,paginationParameters);
            return Ok(result);
        }
    }
}
