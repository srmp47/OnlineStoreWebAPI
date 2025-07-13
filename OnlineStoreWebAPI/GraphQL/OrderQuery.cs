using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserQuery))]
    public class OrderQuery
    {
        public async Task<IEnumerable<Order>> GetOrders([Service] OrderRepository orderRepository)
        {
            return await orderRepository.getAllOrdersAsync(new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }

        public async Task<Order?> GetOrderById(int id, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.getOrderByOrderIdAsync(id);
        }

        public async Task<bool> IsOrderExists(int id, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.isThereOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.getAllOrdersOfUserByIdAsync(userId, new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.getAllOrderItemsByOrderIdAsync(orderId);
        }
    }
} 