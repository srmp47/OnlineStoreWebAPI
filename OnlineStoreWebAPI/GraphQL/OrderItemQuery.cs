using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserQuery))]
    public class OrderItemQuery
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<OrderItem>> GetOrderItems
            ([Service] OrderItemRepository orderItemRepository,int pageId=1,int pageSize = 5)
        {
            return await orderItemRepository.getAllOrderItemsAsync(new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize });
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<OrderItem?> GetOrderItemById(int id, [Service] OrderItemRepository orderItemRepository)
        {
            return await orderItemRepository.getOrderItemByOrderItemId(id);
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsOrderItemExists(int id, [Service] OrderItemRepository orderItemRepository)
        {
            return await orderItemRepository.isThereOrderItemById(id);
        }

        [Authorize(Roles = new[] { "Admin" })]
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