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
        public async Task<IEnumerable<User>> GetUsers([Service] UserRepository userRepository,
            int pageId= 1,int pageSize=5)
        {
            return await userRepository.getAllUsersAsync
                (new Pagination.PaginationParameters { PageId = pageId, PageSize = pageSize });
        }
        [Authorize(Roles = new[] { "Admin" })]

        public async Task<User?> GetUserById(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.getUserByIdAsync(id);
        }
        [Authorize]
        public async Task<User> getMyInformation([Service] UserRepository userRepository, ClaimsPrincipal claims)
        {
            int userId = int.Parse(claims.FindFirst("userId")?.Value ?? "0");
            var user = await userRepository.getUserByIdAsync(userId);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {userId} not found.you don't exist!! how could you login?!");
            }
            return user;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsActiveUser(int id, [Service] UserRepository userRepository)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            return user.isActive;
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> IsUserExists(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.isThereUserWithIdAsync(id);
        }
         
    }
} 