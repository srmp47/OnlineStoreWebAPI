using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController:ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;

        public OrderController(IMapper mapper, IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            this.mapper = mapper;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
        }
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> cancelOrderById(int id)
        {
            if (!await orderRepository.isThereOrderByIdAsync(id)) return NotFound("Order not exist");
            await orderRepository.cancelOrderByIdAsync(id);
            return Ok("Cancelled successfully");
        }
        [HttpPost("AddOrder")]
        public async Task<IActionResult> createNewOrder(OrderDTO inputOrder)
        {
            if (inputOrder == null) return BadRequest("input Order is null!");
            if (!ModelState.IsValid) return BadRequest("Bad Request");
            Order order = mapper.Map<Order>(inputOrder);
            orderRepository.setUserInOrder(order);
            if (inputOrder.orderItemDTOs.Count!=0 && inputOrder.orderItemDTOs != null )
            {
                foreach (var orderItemDTO in inputOrder.orderItemDTOs)
                {
                    var isValidProductId = await productRepository.
                        isThereProductWithIdAsync(orderItemDTO.productId);
                    if (!isValidProductId) return BadRequest("Product not exist");
                    var orderItem = mapper.Map<OrderItem>(orderItemDTO);
                    orderItem.Order = order;
                    orderRepository.setOrderAndProductInOrderItem(orderItem);
                    order.orderItems.Add(orderItem);
                }
            }
            var result = await orderRepository.createNewOrderAsync(order);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> getAllOrders()
        {
            var result = await orderRepository.getAllOrdersAsync();
            if (result == null) return NoContent();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getOrderById(int id)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);

        }
        [HttpGet("Orders of User/{userId}")]
        public async Task<IActionResult> getAllOrdersOfUserById(int userId)
        {
            var orders = await orderRepository.getAllOrdersOfUserByIdAsync( userId);
            if (orders == null) return NotFound();
            return Ok(orders);
        }
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereOrderWithId(int id)
        {
            if (await orderRepository.isThereOrderByIdAsync(id)) return Ok("There is");
            else return Ok("There is not");

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteOrderById(int id)
        {
            var isValidId = await orderRepository.isThereOrderByIdAsync(id);
            if (!isValidId) return NotFound("Product not exist");
            var result = await orderRepository.deleteOrderByIdAsync(id);
            return Ok(result);

        }
        [HttpGet("OrderItemsOfOrder/{orderId}")]
        public async Task<IActionResult> getAllOrderItemsByOrderId(int orderId)
        {
            var isValidId = await orderRepository.isThereOrderByIdAsync(orderId);
            if (!isValidId) return BadRequest("Order not exist");
            var result = orderRepository.getAllOrderItemsByOrderIdAsync(orderId);
            return Ok(result);
        }
        // implement Update function...
    }
}
