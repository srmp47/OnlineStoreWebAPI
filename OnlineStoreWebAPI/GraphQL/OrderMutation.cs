using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]

    public class OrderMutation
    {
        [Authorize]
        public async Task<Order> CreateOrder
            (ClaimsPrincipal claims ,OrderDTO inputOrder, [Service] OrderService orderService,
            [Service] AutoMapper.IMapper mapper, [Service]ProductService productRepository,
            [Service]UserService userRepository)
        {
            if (inputOrder == null) throw new GraphQLException("enter input!");
            var order = mapper.Map<Order>(inputOrder);
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            var context = new ValidationContext(inputOrder);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(inputOrder, context, results, validateAllProperties: true))
            {
                var messages = results.Select(r => r.ErrorMessage);
                throw new GraphQLException(string.Join(" | ", messages));
            }
            var isValidUserId = await userRepository.isThereUserWithIdAsync(userId);
            if (!isValidUserId) throw new GraphQLException("User not found");
            if(inputOrder.orderItemDTOs == null || inputOrder.orderItemDTOs.Count == 0)
                throw new GraphQLException("You must have at least one order item in your order");
            await orderService.setUserInOrder(order, userId);
            foreach (var orderItemDTO in inputOrder.orderItemDTOs)
            {
                var product = await productRepository.getProductByIdAsync(orderItemDTO.productId);
                if (product == null) throw new GraphQLException($"Product with id {orderItemDTO.productId} not exist");
                if (product.StockQuantity < orderItemDTO.quantity)
                    throw new GraphQLException($"There is not enough stock for product with id {product.productId}");
                if (orderItemDTO.quantity <= 0)
                    throw new GraphQLException("You can not add order item with zero or negative quantity");
            }
            foreach (var orderItemDTO in inputOrder.orderItemDTOs)
            {
                var orderItem = mapper.Map<OrderItem>(orderItemDTO);
                orderItem.Order = order;
                await orderService.setOrderAndProductInOrderItem(orderItem);
                order.orderItems.Add(orderItem);
            }
            orderService.setPricesOfOrderItems(order);
            return await orderService.createNewOrderAsync(order);
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
        public async Task<bool> DeleteOrder(int id, [Service] OrderService orderRepository)
        {
            var order = await orderRepository.getOrderByOrderIdAsync(id);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {id} not found.");
            }
            return await orderRepository.deleteOrderByIdAsync(id);
        }
        [Authorize]
        public async Task<Order> changeMyOrderStatusByOrderId
            (int orderId,OrderStatus status ,[Service] OrderService orderRepository,
            ClaimsPrincipal claims, [Service]ProductService productRepository)
        {
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            var order = await orderRepository.getOrderByOrderIdAsync(orderId);
            if (order == null)
            {
                throw new GraphQLException($"Order with ID {orderId} not found.");
            }
            if(order.userId != userId)
                throw new GraphQLException("You can not cancel this order, because you are not owner of this order!");
            if (order.status == OrderStatus.Cancelled)
            {
                throw new GraphQLException("This order is already cancelled.");
            }
            if (status == OrderStatus.Cancelled && order.status != OrderStatus.Pending)
            {
                throw new GraphQLException("The order is being prepared for shipment. you can not cancel it");
            }
            if (status == OrderStatus.Cancelled && order.status == OrderStatus.Pending)
            {
                return await orderRepository.changeOrderStatusByOrderIdAsync(order.OrderId, status);
            }
            if (status == OrderStatus.Processing && order.status == OrderStatus.Pending)
            {
                foreach (var orderItem in order.orderItems)
                {
                    var productQuantity = await productRepository.getQuantityOfProduct(orderItem.productId);
                    if (orderItem.quantity > productQuantity)
                        throw new GraphQLException
                            ($"We only have {productQuantity} units of “{orderItem.productId}” in stock, " +
                            $"but you requested {orderItem.quantity}. Please reduce the quantity");
                }
                foreach (var orderItem in order.orderItems)
                {
                    await productRepository.removeFromStockQuantity(orderItem.productId, orderItem.quantity);
                }
                return await orderRepository.changeOrderStatusByOrderIdAsync(order.OrderId, status);
            }
            else throw new GraphQLException("You can not change the status of this order to this status");
        }
        [Authorize(Roles = new[] { "Admin" })]
        // in this method I do not remove the quantity of order items from the stock qunatity of product
        // TODO correct these methodes (quantity is not changed)
        public async Task<Order> ChangeOrderStatus(int id, OrderStatus status, [Service] OrderService orderRepository)
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