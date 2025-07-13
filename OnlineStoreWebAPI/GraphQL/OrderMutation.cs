using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]

    public class OrderMutation
    {
        public async Task<Order> CreateOrder
            (OrderDTO inputOrder, [Service] OrderRepository orderRepository,
            [Service] AutoMapper.IMapper mapper, [Service]ProductRepository productRepository,
            [Service]UserRepository userRepository)
        {
            if (inputOrder == null) throw new GraphQLException("enter input!");
            var order = mapper.Map<Order>(inputOrder);
            var isValidUserId = await userRepository.isThereUserWithIdAsync(inputOrder.userId);
            if (!isValidUserId) throw new GraphQLException("User not found");
            await orderRepository.setUserInOrder(order);
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

        public async Task<Order> DeleteOrder(int id, [Service] OrderRepository orderRepository)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {id} not found.");
            }

            return await orderRepository.deleteOrderByIdAsync(id);
        }

        public async Task<Order> CancelOrder(int id, [Service] OrderRepository orderRepository)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {id} not found.");
            }

            await orderRepository.cancelOrderByIdAsync(id);
            return await orderRepository.getOrderByOrderIdAsync(id);
        }

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