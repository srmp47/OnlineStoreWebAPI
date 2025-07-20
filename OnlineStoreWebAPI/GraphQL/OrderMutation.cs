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
                    var product = await productRepository.getProductByIdAsync(orderItemDTO.productId);
                    if(product == null)
                            throw new GraphQLException
                            ($"product with id {orderItemDTO.productId} not exist!");
                    if (product.StockQuantity < orderItemDTO.quantity)
                        throw new GraphQLException("There is not enough stock for this product");
                    var orderItem = mapper.Map<OrderItem>(orderItemDTO);
                    orderItem.Order = order;
                    await orderRepository.setOrderAndProductInOrderItem(orderItem);
                    await productRepository.removeFromStockQuantity(orderItem.productId,orderItem.quantity);
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
        // in this method I do not remove the quantity of order items from the stock qunatity of product
        // because I assume that admin deletes the orders just when order is cancelled
        // and in that case the stock quantity of products is already added back to the stock quantity
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
            (int orderId,[Service] OrderRepository orderRepository,
            ClaimsPrincipal claims, [Service]ProductRepository productRepository)
        {
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            var order = await orderRepository.getOrderByOrderIdAsync(orderId);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {orderId} not found.");
            }
            if(order.userId != userId)
                throw new GraphQLException("You can not cancel this order, because you are not owner of this order!");
            if (order.status == OrderStatus.Shipped || order.status == OrderStatus.Delivered)
                throw new GraphQLException("You can not cancel this order[shipped or delivered]");
            if (order.status == OrderStatus.Cancelled)
            {
                throw new GraphQLException("This order is already cancelled.");
            }
            foreach (var orderItem in order.orderItems)
            {
                await productRepository.addToStockQuantity(orderItem.productId, orderItem.quantity);
            }
           
            await orderRepository.cancelOrderByIdAsync(orderId);
            var result = await orderRepository.getOrderByOrderIdAsync(orderId);
            return result;
        }
        [Authorize(Roles = new[] { "Admin" })]
        // in this method I do not remove the quantity of order items from the stock qunatity of product
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