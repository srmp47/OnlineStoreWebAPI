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
        public OrderController(IMapper mapper, IOrderRepository orderRepository)
        {
            this.mapper = mapper;
            this.orderRepository = orderRepository;
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
            foreach(var orderItemDTO in inputOrder.orderItemDTOs)
            {
                var orderItem = mapper.Map<OrderItem>(orderItemDTO);
                order.orderItems.Add(orderItem);
            }
            var result = await orderRepository.createNewOrderAsync(order);
            return Ok(result);
        }
    }
}
