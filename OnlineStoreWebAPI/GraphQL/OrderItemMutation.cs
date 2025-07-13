using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserMutation))]
    public class OrderItemMutation
    {
        public async Task<OrderItem> CreateOrderItem
            (int orderId,OrderItemDTO input, [Service] OrderItemRepository orderItemRepository,
            [Service] AutoMapper.IMapper mapper, [Service]OrderRepository orderRepository,
            [Service]ProductRepository productRepository)
        {
            if (input == null) throw new GraphQLException("input product is null!");
            var isValidOrderId = await orderRepository.isThereOrderByIdAsync(orderId);
            var isValidProductId = await productRepository.
                isThereProductWithIdAsync(input.productId);
            if (!isValidProductId) throw new GraphQLException("Invalid product!");
            var orderItem = mapper.Map<OrderItem>(input);
            orderItem.OrderId = orderId;
            await orderItemRepository.setOrderAndProductInOrderItem(orderItem);
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

        public async Task<OrderItem> DeleteOrderItem
            (int id, [Service] OrderItemRepository orderItemRepository)
        {
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(id);
            if (orderItem == null)
            {
                throw new GraphQLException($"OrderItem with ID {id} not found.");
            }

            return await orderItemRepository.deleteOrderItemByIdAsync(id);
        }

        public async Task<OrderItem> ChangeOrderItemQuantity(int id, int quantity, [Service] OrderItemRepository orderItemRepository)
        {
            var orderItem = await orderItemRepository.getOrderItemByOrderItemId(id);
            if (orderItem == null)
            {
                throw new GraphQLException($"OrderItem with ID {id} not found.");
            }
            if (quantity < 0) throw new GraphQLException("invalid quantity!");

            return await orderItemRepository.changeQuantityByOrderItemId(id, quantity);
        }

        public async Task<bool> isThereOrderItemWithId(int id, [Service] OrderItemRepository orderItemRepository)
        {
            if (await orderItemRepository.isThereOrderItemById(id)) return true;
            else return false;

        }
    }
} 