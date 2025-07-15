using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.Security.Claims;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]

    public class OrderMutation
    {
        [Authorize]
        public async Task<Order> CreateOrder
            (ClaimsPrincipal claims ,OrderDTO inputOrder, [Service] OrderRepository orderRepository,
            [Service] AutoMapper.IMapper mapper, [Service]ProductRepository productRepository,
            [Service]UserRepository userRepository)
        {
            if (inputOrder == null) throw new GraphQLException("enter input!");
            var order = mapper.Map<Order>(inputOrder);
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);

            var isValidUserId = await userRepository.isThereUserWithIdAsync(userId);
            if (!isValidUserId) throw new GraphQLException("User not found");
            await orderRepository.setUserInOrder(order, userId);
            if (inputOrder.orderItemDTOs != null && inputOrder.orderItemDTOs.Count != 0 )
            {
                foreach (var orderItemDTO in inputOrder.orderItemDTOs)
                {
                    var isValidProductId = await productRepository.
                        isThereProductWithIdAsync(orderItemDTO.productId);
                    if (!isValidProductId) throw new GraphQLException
                            ($"product with id {orderItemDTO.productId} not exist!");
                    var orderItem = mapper.Map<OrderItem>(orderItemDTO);
                    orderItem.Order = order;
                    await orderRepository.setOrderAndProductInOrderItem(orderItem);
                    order.orderItems.Add(orderItem);
                }
            }
            return await orderRepository.createNewOrderAsync(order);
        }

        //public async Task<Order> UpdateOrder(int orderId, OrderUpdateInputType input, [Service] OrderRepository orderRepository, [Service] AutoMapper.IMapper mapper)
        //{
        //    var existingOrder = await orderRepository.getOrderByOrderIdAsync(orderId);
        //    if (existingOrder == null)
        //    {
        //        throw new GraphQLException($"Order with ID {orderId} not found.");
        //    }

        //    var order = mapper.Map<Order>(input);
        //    order.OrderId = orderId;
        //    return await orderRepository.updateOrderAsync(order);
        //}
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Order> DeleteOrder(int id, [Service] OrderRepository orderRepository)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {id} not found.");
            }

            return await orderRepository.deleteOrderByIdAsync(id);
        }
        [Authorize]
        public async Task<Order> CancelOrder
            (int orderId,[Service] OrderRepository orderRepository,ClaimsPrincipal claims)
        {
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            var order = await orderRepository.getOrderByOrderIdAsync(orderId);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {userId} not found.");
            }
            if(order.userId != userId)
                throw new GraphQLException("You can not cancel this order, because you are not owner of this order!");

            await orderRepository.cancelOrderByIdAsync(orderId);
            return await orderRepository.getOrderByOrderIdAsync(orderId);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Order> ChangeOrderStatus(int id, OrderStatus status, [Service] OrderRepository orderRepository)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {id} not found.");
            }

            return await orderRepository.changeOrderStatusByOrderIdAsync(id, status);
        }
    }
} 