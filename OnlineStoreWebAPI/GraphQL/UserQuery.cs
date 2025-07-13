using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.GraphQL
{
    public class UserQuery
    {
        public async Task<IEnumerable<User>> GetUsers([Service] UserRepository userRepository)
        {
            return await userRepository.getAllUsersAsync(new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }

        public async Task<User?> GetUserById(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.getUserByIdAsync(id);
        }
        public async Task<bool> IsActiveUser(int id, [Service] UserRepository userRepository)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            return user.isActive;
        }

        public async Task<bool> IsUserExists(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.isThereUserWithIdAsync(id);
        }
    }
} 