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
        // TODO we should return IQueryable in graphql outputs....
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<Order>> GetOrders
            ([Service] OrderService orderService,int pageId = 1, int pageSize = 5)
        {
            return await orderService.getAllOrdersAsync
                (new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize });
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Order?> GetOrderById(int id, [Service] OrderService orderService)
        {
            return await orderService.getOrderByOrderIdAsync(id);
        }
        [Authorize]
        public async Task<IEnumerable<Order>> getAllOrdersOfCurrentUser
            ([Service] OrderService orderService, [Service] UserService userRepository,
            ClaimsPrincipal claims , int pageId = 1 , int pageSize = 5)
        {
            var claimId = claims.Claims.FirstOrDefault(u => u.Type == "userId")?.Value;
            int currentUserId = Convert.ToInt32(claimId);
            var isValidUserId = await userRepository.isThereUserWithIdAsync(currentUserId);
            if (!isValidUserId) throw new GraphQLException("User not found");
            return (await orderService.getAllOrdersOfUserByIdAsync
                (currentUserId, new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize }));
;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsOrderExists(int id, [Service] OrderService orderService)
        {
            return await orderService.isThereOrderByIdAsync(id);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<Order>> GetOrdersByUserId
            (int userId, [Service] OrderService orderService,int pageId=1,int pageSize = 5)
        {
            return await orderService.getAllOrdersOfUserByIdAsync
                (userId, new Pagination.PaginationParameters {PageId = pageId, PageSize = pageSize });
        }
        
    }
} 