using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using System.Security.Claims;

namespace OnlineStoreWebAPI.GraphQL
{
    [ExtendObjectType(typeof(UserQuery))]
    public class OrderItemQuery
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<OrderItem>> GetOrderItems
            ([Service] OrderItemService orderItemService,int pageId=1,int pageSize = 5)
        {
            return await orderItemService.getAllOrderItemsAsync(new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize });
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<OrderItem?> GetOrderItemById(int id, [Service] OrderItemService orderItemService)
        {
            return await orderItemService.getOrderItemByOrderItemId(id);
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsOrderItemExists(int id, [Service] OrderItemService orderItemService)
        {
            return await orderItemService.isThereOrderItemById(id);
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId, [Service] OrderItemService orderItemService ,
             [Service] OrderService orderService, int pageId = 1, int pageSize = 5)
        {
            var isValidOrederId = await orderService.isThereOrderByIdAsync(orderId);
            if (!isValidOrederId)
            {
                throw new GraphQLException($"Order with ID {orderId} not found.");
            }
            return await orderItemService.getAllOrderItemsByOrderIdAsync(orderId,new PaginationParameters { PageSize = pageSize , PageId = pageId});
        }
        [Authorize]
        public async Task<IEnumerable<OrderItem>> getAllOrderItemsOfMyOrder(int orderId , [Service] OrderItemService orderItemService,
             [Service] OrderService orderService , ClaimsPrincipal claims, int pageId = 1, int pageSize = 5)
        {
            var isValidOrederId = await orderService.isThereOrderByIdAsync(orderId);
            if (!isValidOrederId)
            {
                throw new GraphQLException($"Order with ID {orderId} not found.");
            }
            var claimId = claims.Claims.FirstOrDefault(u => u.Type == "userId")?.Value;
            var currentUserId = Convert.ToInt32(claimId);
            var userId = await orderService.getUserIdOfOrder(orderId);
            if(userId != currentUserId) throw new GraphQLException("You can not see this order items, this is not your order");
            return await orderItemService.getAllOrderItemsByOrderIdAsync(orderId, new PaginationParameters { PageId = pageId , PageSize = pageSize});
        }

    }
} 