using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/Order")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly IUserRepository userRepository;

        public OrderController(IMapper mapper, IOrderRepository orderRepository,
            IProductRepository productRepository, IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
            this.userRepository = userRepository;
            this.userRepository = userRepository;
        }
        [HttpPatch("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> cancelMyOrder(int id)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var isValidId = await orderRepository.isThereOrderByIdAsync(id);
            if (!isValidId) return BadRequest("Order not exist");
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null || order.userId != currentUserId)
                return BadRequest("You can not cancel this order");
            if (order.status == OrderStatus.Shipped || order.status == OrderStatus.Delivered)
                return BadRequest("You can not cancel this order[shipped or delivered]");
            if (order.status == OrderStatus.Cancelled)
            {
                throw new GraphQLException("This order is already cancelled.");
            }
            await orderRepository.cancelOrderByIdAsync(id);
            foreach(var orderItem in order.orderItems)
            {
                await productRepository.addToStockQuantity(orderItem.productId, orderItem.quantity);
            }
            return Ok("Your order has been cancelled successfully!");
        }
        [Authorize]
        [HttpPost("AddOrder")]
        public async Task<IActionResult> createNewOrder(OrderDTO inputOrder)
        {
            if (inputOrder == null) return BadRequest("input Order is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            Order order = mapper.Map<Order>(inputOrder);
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            await orderRepository.setUserInOrder(order, currentUserId);
            // TODO this section should be atomic!
            if (inputOrder.orderItemDTOs != null && inputOrder.orderItemDTOs.Count != 0)
            {
                foreach (var orderItemDTO in inputOrder.orderItemDTOs)
                {
                    var product = await productRepository.getProductByIdAsync(orderItemDTO.productId);
                    if (product == null) return BadRequest("Product not exist");
                    if (product.StockQuantity < orderItemDTO.quantity)
                        return BadRequest("There is not enough stock for this product");
                    var orderItem = mapper.Map<OrderItem>(orderItemDTO);
                    orderItem.Order = order;
                    await orderRepository.setOrderAndProductInOrderItem(orderItem);
                    order.orderItems.Add(orderItem);
                    await productRepository.removeFromStockQuantity(orderItemDTO.productId, orderItemDTO.quantity);
                }
            }

            var result = await orderRepository.createNewOrderAsync(order);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> getAllOrders
            ([FromQuery] PaginationParameters paginationParameters)
        {
            var result = await orderRepository.getAllOrdersAsync(paginationParameters);
            if (result == null) return NoContent();
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getOrderById(int id)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);

        }
        [HttpGet("Orders of User/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getAllOrdersOfUserById
            (int userId, [FromQuery] PaginationParameters paginationParameters)
        {
            var orders = await orderRepository.getAllOrdersOfUserByIdAsync(userId, paginationParameters);
            if (orders == null) return NotFound();
            return Ok(orders);
        }
        [HttpGet("show my orders")]
        [Authorize]
        public async Task<IActionResult> getAllOrdersOfCurrentUser
            ([FromQuery] PaginationParameters paginationParameters)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var orders = await orderRepository.getAllOrdersOfUserByIdAsync(currentUserId, paginationParameters);
            if (orders == null) return NotFound();
            return Ok(orders);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereOrderWithId(int id)
        {
            if (await orderRepository.isThereOrderByIdAsync(id)) return Ok("There is");
            else return Ok("There is not");

        }
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        // in this method I do not remove the quantity of order items from the stock qunatity of product
        // because I assume that admin deletes the orders just when order is cancelled
        // and in that case the stock quantity of products is already added back to the stock quantity
        public async Task<IActionResult> deleteOrderById(int id)
        {
            var isValidId = await orderRepository.isThereOrderByIdAsync(id);
            if (!isValidId) return NotFound("Order not exist");
            var result = await orderRepository.deleteOrderByIdAsync(id);
            return Ok(result);

        }

        [HttpGet("OrderItemsOfOrder/{orderId}")]
        [Authorize("Admin")]
        //TODO user can only see his/her order items.
        public async Task<IActionResult> getAllOrderItemsByOrderId(int orderId)
        {
            var isValidId = await orderRepository.isThereOrderByIdAsync(orderId);
            if (!isValidId) return BadRequest("Order not exist");
            var result = orderRepository.getAllOrderItemsByOrderIdAsync(orderId);
            return Ok(result);
        }
        [HttpGet("OrderItemsOfMyOrder/{orderId}")]
        [Authorize]
        public async Task<IActionResult> getAllOrderItemsOfMyOrder(int orderId)
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int currentUserId = Convert.ToInt32(claimId.Value);
            var isValidId = await orderRepository.isThereOrderByIdAsync(orderId);
            if (!isValidId) return BadRequest("Order not exist");
            var order = await orderRepository.getOrderByOrderIdAsync(orderId);
            if (order == null || order.userId != currentUserId) return BadRequest("You can not see this order");
            var result = await orderRepository.getAllOrderItemsByOrderIdAsync(orderId);
            return Ok(result);
        }
        
        [HttpPatch("{id}/changeStatus/{status}")]
        [Authorize(Roles = "Admin")]
        // in this method I do not remove the quantity of order items from the stock qunatity of product
        public async Task<IActionResult> changeOrderStatusByOrderId(int id, OrderStatus status)
        {
            var isValidId = await orderRepository.isThereOrderByIdAsync(id);
            if (!isValidId) return BadRequest("Order not exist");
            var result = await orderRepository.changeOrderStatusByOrderIdAsync(id, status);
            return Ok(result);
        }
        



     }
}
