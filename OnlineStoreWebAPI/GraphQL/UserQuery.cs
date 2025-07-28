using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using System.Security.Claims;

namespace OnlineStoreWebAPI.GraphQL
{
    public class UserQuery
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<IEnumerable<User>> GetUsers([Service] UserService userService,
            int pageId= 1,int pageSize=5)
        {
            return await userService.getAllUsersAsync
                (new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize });
        }
        [Authorize(Roles = new[] { "Admin" })]

        public async Task<User?> GetUserById(int id, [Service] UserService userService)
        {
            return await userService.getUserByIdAsync(id);
        }
        [Authorize]
        public async Task<User> getMyInformation([Service] UserService userService, ClaimsPrincipal claims)
        {
            int userId = int.Parse(claims.FindFirst("userId")?.Value ?? "0");
            var user = await userService.getUserByIdAsync(userId);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {userId} not found.you don't exist!! how could you login?!");
            }
            return user;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsActiveUser(int id, [Service] UserService userService)
        {
            var user = await userService.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            return user.isActive;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsUserExists(int id, [Service] UserService userService)
        {
            return await userService.isThereUserWithIdAsync(id);
        }
         
    }
} 