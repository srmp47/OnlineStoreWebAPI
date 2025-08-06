using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using OnlineStoreWebAPI.Enum;
using Castle.Core.Internal;
using Microsoft.IdentityModel.Tokens;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/Order")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly IUserService userService;

        public OrderController(IMapper mapper, IOrderService orderService,
            IProductService productService, IUserService userService)
        {
            this.mapper = mapper;
            this.orderService = orderService;
            this.productService = productService;
            this.userService = userService;
            this.userService = userService;
        }
        
        [Authorize]
        [HttpPost("AddOrder")]
        public async Task<IActionResult> createNewOrder(OrderDTO inputOrder)
        {
            if( inputOrder.orderItemDTOs == null || inputOrder.orderItemDTOs.Count == 0)
                return BadRequest("you must have at least one order item in your order");
            Order order = mapper.Map<Order>(inputOrder);
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            await orderService.setUserInOrder(order, currentUserId);
            foreach (var orderItemDTO in inputOrder.orderItemDTOs)
            {
            var product = await productService.getProductByIdAsync(orderItemDTO.productId);
            if (product == null) return BadRequest($"Product with id {orderItemDTO.productId} not exist");
            if (product.StockQuantity < orderItemDTO.quantity)
            return BadRequest($"There is not enough stock for product with id {product.productId}");
            if(orderItemDTO.quantity <= 0)
                    return BadRequest("You can not add order item with zero or negative quantity");
            }
            foreach (var orderItemDTO in inputOrder.orderItemDTOs)
            {
            var orderItem = mapper.Map<OrderItem>(orderItemDTO);
            orderItem.Order = order;
            await orderService.setOrderAndProductInOrderItem(orderItem);
            order.orderItems.Add(orderItem);
            }
            orderService.setPricesOfOrderItems(order);
            

            var result = await orderService.createNewOrderAsync(order);
            return CreatedAtAction(nameof(getOrderById), new { id = result.OrderId }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> getAllOrders
            ([FromQuery] PaginationParameters paginationParameters)
        {
            var result = await orderService.getAllOrdersAsync(paginationParameters);
            if (!result.Any()) return NoContent();
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getOrderById(int id)
        {
            var order = await orderService.getOrderByOrderIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);

        }
        [HttpGet("Orders of User/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getAllOrdersOfUserById
            (int userId, [FromQuery] PaginationParameters paginationParameters)
        {
            var isValidUserId = await userService.isThereUserWithIdAsync(userId);
            if(!isValidUserId) return NotFound("user not found");
            var orders = await orderService.getAllOrdersOfUserByIdAsync(userId, paginationParameters);
            if (orders.Any() == false) return NoContent();
            return Ok(orders);
        }
        [HttpGet("show my orders")]
        [Authorize]
        public async Task<IActionResult> getAllOrdersOfCurrentUser
            ([FromQuery] PaginationParameters paginationParameters)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var orders = await orderService.getAllOrdersOfUserByIdAsync(currentUserId, paginationParameters);
            if (orders.Any() == false) return NoContent();
            return Ok(orders);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereOrderWithId(int id)
        {
            if (await orderService.isThereOrderByIdAsync(id)) return Ok("There is");
            else return NotFound("There is not");

        }
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        // in this method I do not remove the quantity of order items from the stock qunatity of product
        // because I assume that admin deletes the orders just when order is cancelled
        // and in that case the stock quantity of products is already added back to the stock quantity
        public async Task<IActionResult> deleteOrderById(int id)
        {
            var isValidId = await orderService.isThereOrderByIdAsync(id);
            if (!isValidId) return NotFound("Order not exist");
            var result = await orderService.deleteOrderByIdAsync(id);
            return Ok(result);

        }

        
        
        [HttpPatch("{id}/changeStatus/{status}")]
        [Authorize(Roles = "Admin")]
        // in this method I do not remove the quantity of order items from the stock qunatity of product
        public async Task<IActionResult> changeOrderStatusByOrderId(int id, OrderStatus status)
        {
            var isValidId = await orderService.isThereOrderByIdAsync(id);
            if (!isValidId) return BadRequest("Order not exist");
            var result = await orderService.changeOrderStatusByOrderIdAsync(id, status);
            return Ok(result);
        }

        [HttpPatch("{id}/changeStatusOfMyOrder/{status}")]
        [Authorize]
        public async Task<IActionResult> changeMyOrderStatusByOrderId(int id, OrderStatus status)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var isValidId = await orderService.isThereOrderByIdAsync(id);
            if (!isValidId) return BadRequest("Order not exist");
            var order = await orderService.getOrderByOrderIdAsync(id);
            if (order == null || order.userId != currentUserId)
                return BadRequest("You can not change the status of this order");
            if ( order.status == OrderStatus.Cancelled)
            {
                return BadRequest("This order is already cancelled.");
            }
            if (status == OrderStatus.Cancelled && order.status != OrderStatus.Pending)
            {
                return BadRequest("You can not cancel this order, it is not pending.");
            }
            if(status == OrderStatus.Cancelled && order.status == OrderStatus.Pending)
            {
                var result = await orderService.changeOrderStatusByOrderIdAsync(id, status);
                return Ok("The order is cancelled successfully.");
            }
            if (status == OrderStatus.Processing && order.status == OrderStatus.Pending)
            {
                foreach (var orderItem in order.orderItems)
                {
                    var productQuantity = await productService.getQuantityOfProduct(orderItem.productId);
                    if (orderItem.quantity > productQuantity)
                        return BadRequest
                            ($"We only have {productQuantity} units of “{orderItem.productId}” in stock, " +
                            $"but you requested {orderItem.quantity}. Please reduce the quantity");
                }
                foreach (var orderItem in order.orderItems)
                {
                    await productService.removeFromStockQuantity(orderItem.productId, orderItem.quantity);
                }
                var result = await orderService.changeOrderStatusByOrderIdAsync(id, status);
                return Ok("The order is processing successfully.");
            }
            else return BadRequest("You can not change the status of this order to this status");
            
        }





        }
}
