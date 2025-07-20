using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.Security.Claims;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]
    public class OrderItemMutation
    {
        [Authorize]
        public async Task<OrderItem> CreateOrderItem
            (int orderId,OrderItemDTO input, [Service] OrderItemRepository orderItemRepository,
            [Service] AutoMapper.IMapper mapper, [Service]OrderRepository orderRepository,
            [Service]ProductRepository productRepository,ClaimsPrincipal claims)
        {
            if (input == null) throw new GraphQLException("input product is null!");
            var isValidOrderId = await orderRepository.isThereOrderByIdAsync(orderId);
            if(!isValidOrderId) throw new GraphQLException("Invalid order!");
            var product = await productRepository.getProductByIdAsync(input.productId);
            if(product == null)
                throw new GraphQLException($"Product with ID {input.productId} not found.");
            if (product.StockQuantity < input.quantity)
                throw new GraphQLException("There is not enough stock for this product");
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            var order = await  orderRepository.getOrderByOrderIdAsync(orderId);
            if(order.userId != userId)
            {
                throw new GraphQLException("You can not add item to this order,this order is not for you");
            }
            if(input.quantity == 0) throw new GraphQLException("You can not add order item with zero quantity");
            var orderItem = mapper.Map<OrderItem>(input);
            orderItem.OrderId = orderId;
            await orderItemRepository.setOrderAndProductInOrderItem(orderItem);
            await productRepository.removeFromStockQuantity(orderItem.productId,orderItem.quantity);
            return await orderItemRepository.createNewOrderItemAsync(orderItem);
        }

        //public async Task<OrderItem> UpdateOrderItem(int orderItemId, OrderItemUpdateInputType input, [Service] OrderItemRepository orderItemRepository, [Service] AutoMapper.IMapper mapper)
        //{
        //    var existingOrderItem = await orderItemRepository.getOrderItemByOrderItemId(orderItemId);
        //    if (existingOrderItem == null)
        //    {
        //        throw new GraphQLException($"OrderItem with ID {orderItemId} not found.");
        //    }

        //    var orderItem = mapper.Map<OrderItem>(input);
        //    orderItem.OrderItemId = orderItemId;
        //    return await orderItemRepository.updateOrderItemAsync(orderItem);
        //}
        [Authorize]
        public async Task<OrderItem> DeleteOrderItem
            (int orderItemId, [Service] OrderItemRepository orderItemRepository,
            ClaimsPrincipal claims, [Service]ProductRepository productRepository)
        {
            var isValidOrderItemId = await orderItemRepository.isThereOrderItemById(orderItemId);
            if(!isValidOrderItemId)
                throw new GraphQLException($"OrderItem with ID {orderItemId} not found.");
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(orderItemId);
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            if(orderItem.Order.userId != userId)
                throw new GraphQLException("You can not delete this order item, it is not for you");
            
            await productRepository.addToStockQuantity(orderItem.productId, orderItem.quantity);
            return await orderItemRepository.deleteOrderItemByIdAsync(orderItemId);
        }
        [Authorize]
        public async Task<OrderItem> ChangeOrderItemQuantity
            (int orderItemId, int quantity, [Service] OrderItemRepository orderItemRepository,
            ClaimsPrincipal claims, [Service]ProductRepository productRepository)
        {
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(orderItemId);
            if (orderItem == null)
            {
                throw new GraphQLException($"OrderItem with ID {orderItemId} not found.");
            }
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            if (orderItem.Order.userId != userId)
            {
                throw new GraphQLException("You can not change this order item, it is not for you");
            }
            if(orderItem.Order.status == OrderStatus.Cancelled)
            {
                throw new GraphQLException("You can not change this order item, this order is cancelled");
            }
            if (quantity < 0 || quantity > orderItem.Product.StockQuantity + orderItem.quantity ) 
                throw new GraphQLException("invalid quantity!");
            await productRepository.setStockQuantity(orderItem.productId, orderItem.Product.StockQuantity + orderItem.quantity - quantity);
            return await orderItemRepository.changeQuantityByOrderItemId
                (orderItemId, quantity);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> isThereOrderItemWithId(int id, [Service] OrderItemRepository orderItemRepository)
        {
            if (await orderItemRepository.isThereOrderItemById(id)) return true;
            else return false;

        }
    }
} 