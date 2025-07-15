using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.Security.Claims;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserQuery))]
    public class OrderQuery
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<Order>> GetOrders([Service] OrderRepository orderRepository)
        {
            return await orderRepository.getAllOrdersAsync(new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Order?> GetOrderById(int id, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.getOrderByOrderIdAsync(id);
        }
        [Authorize]
        public async Task<Order> getAllOrdersOfCurrentUser
            ([Service] OrderRepository orderRepository, [Service] UserRepository userRepository,ClaimsPrincipal claims)
        {
            var claimId = claims.Claims.FirstOrDefault(u => u.Type == "userId")?.Value;
            int currentUserId = Convert.ToInt32(claimId);
           // var isValidUserId = await userRepository.isThereUserWithIdAsync(currentUserId);
           // if (!isValidUserId) throw new GraphQLException("User not found");
            return ((await orderRepository.getAllOrdersOfUserByIdAsync
                (currentUserId, new Pagination.PaginationParameters { PageId = 1, PageSize = 100 }))
                .FirstOrDefault())!;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsOrderExists(int id, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.isThereOrderByIdAsync(id);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.getAllOrdersOfUserByIdAsync(userId, new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId, [Service] OrderRepository orderRepository)
        {
            return await orderRepository.getAllOrderItemsByOrderIdAsync(orderId);
        }
    }
} 