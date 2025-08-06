using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using OnlineStoreWebAPI.Enum;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]
    public class OrderItemMutation
    {
        [Authorize]
        public async Task<OrderItem> CreateOrderItem
            (int orderId,OrderItemDTO input, [Service] OrderItemService orderItemService,
            [Service] AutoMapper.IMapper mapper, [Service]OrderService orderRepository,
            [Service]ProductService productRepository,ClaimsPrincipal claims)
        {
            var context = new ValidationContext(input);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(input, context, results, validateAllProperties: true))
            {
                var messages = results.Select(r => r.ErrorMessage);
                throw new GraphQLException(string.Join(" | ", messages));
            }
            if (input == null) throw new GraphQLException("input is null!");
            var isValidOrderId = await orderRepository.isThereOrderByIdAsync(orderId);
            if(!isValidOrderId) throw new GraphQLException("Invalid order!");
            var product = await productRepository.getProductByIdAsync(input.productId);
            if(product == null)
                throw new GraphQLException($"Product with ID {input.productId} not found.");
            var order = await orderRepository.getOrderByOrderIdAsync(orderId);
            if (order.status == OrderStatus.Cancelled)
                throw new GraphQLException("You can not add item to this order, this order is cancelled");
            if(order.status != OrderStatus.Cancelled && order.status != OrderStatus.Pending)
                throw new GraphQLException("The order is being prepared for shipment. you can not add new product. please place a new order");
            if (product.StockQuantity < input.quantity)
                throw new GraphQLException("There is not enough stock for this product");
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            if(order.userId != userId)
            {
                throw new GraphQLException("You can not add item to this order,this order is not for you");
            }
            if(input.quantity <= 0) throw new GraphQLException("You can not add order item with zero or negative quantity");
            var orderItem = mapper.Map<OrderItem>(input);
            orderItem.OrderId = orderId;
           // await orderItemService.setOrderAndProductInOrderItem(orderItem);
            return await orderItemService.createNewOrderItemAsync(orderItem);
        }

        //public async Task<OrderItem> UpdateOrderItem(int orderItemId, OrderItemUpdateInputType input, [Service] orderItemService orderItemService, [Service] AutoMapper.IMapper mapper)
        //{
        //    var existingOrderItem = await orderItemService.getOrderItemByOrderItemId(orderItemId);
        //    if (existingOrderItem == null)
        //    {
        //        throw new GraphQLException($"OrderItem with ID {orderItemId} not found.");
        //    }

        //    var orderItem = mapper.Map<OrderItem>(input);
        //    orderItem.OrderItemId = orderItemId;
        //    return await orderItemService.updateOrderItemAsync(orderItem);
        //}
        [Authorize]
        public async Task<bool> DeleteOrderItem
            (int orderItemId, [Service] OrderItemService orderItemService,
            ClaimsPrincipal claims, [Service]ProductService productRepository)
        {
            var isValidOrderItemId = await orderItemService.isThereOrderItemById(orderItemId);
            if(!isValidOrderItemId)
                throw new GraphQLException($"OrderItem with ID {orderItemId} not found.");
            var orderItem = await orderItemService.getOrderItemByOrderItemId(orderItemId);
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            //TODO can you get the user id of order item with better performance?(e.g. without loading the orderItem)
            if (orderItem.Order.userId != userId)
                throw new GraphQLException("You can not delete this order item, it is not for you");
            if(orderItem.Order.status == OrderStatus.Cancelled)
            {
                throw new GraphQLException("You can not delete this order item, this order is cancelled");
            }
            if(orderItem.Order.status != OrderStatus.Cancelled && orderItem.Order.status != OrderStatus.Pending)
            {
                throw new GraphQLException("The order is being prepared for shipment. you can not delete this product.");
            }

            return await orderItemService.deleteOrderItemByIdAsync(orderItemId);
        }
        [Authorize]
        public async Task<OrderItem> ChangeOrderItemQuantity
            (int orderItemId, int quantity, [Service] OrderItemService orderItemService,
            ClaimsPrincipal claims, [Service]ProductService productRepository)
        {
            var orderItem = await orderItemService.getOrderItemByOrderItemId(orderItemId);
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
            if (orderItem.Order.status != OrderStatus.Pending)
            {
                throw new GraphQLException("The order is being prepared for shipment. you can not change this product.");
            }
            if (quantity < 0 || quantity > orderItem.Product.StockQuantity + orderItem.quantity ) 
                throw new GraphQLException("invalid quantity!");
            
            return await orderItemService.changeQuantityByOrderItemId
                (orderItemId, quantity);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> isThereOrderItemWithId(int id, [Service] OrderItemService orderItemService)
        {
            if (await orderItemService.isThereOrderItemById(id)) return true;
            else return false;

        }
    }
} 