using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserQuery))]
    public class OrderItemQuery
    {
        public async Task<IEnumerable<OrderItem>> GetOrderItems([Service] OrderItemRepository orderItemRepository)
        {
            return await orderItemRepository.getAllOrderItemsAsync(new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }

        public async Task<OrderItem?> GetOrderItemById(int id, [Service] OrderItemRepository orderItemRepository)
        {
            return await orderItemRepository.getOrderItemByOrderItemId(id);
        }

        public async Task<bool> IsOrderItemExists(int id, [Service] OrderItemRepository orderItemRepository)
        {
            return await orderItemRepository.isThereOrderItemById(id);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId, [Service] OrderRepository orderRepository)
        {
            var isValidOrederId = await orderRepository.isThereOrderByIdAsync(orderId);
            if (!isValidOrederId)
            {
                throw new GraphQLException($"Order with ID {orderId} not found.");
            }
            return await orderRepository.getAllOrderItemsByOrderIdAsync(orderId);
        }
    }
} 