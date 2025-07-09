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
            // For simplicity, no pagination here
            return await userRepository.getAllUsersAsync(new Pagination.PaginationParameters { PageId = 1, PageSize = 100 });
        }

        public async Task<User?> GetUserById(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.getUserByIdAsync(id);
        }
        public async Task<string> IsActiveUser(int id, [Service] UserRepository userRepository)
        {
            // First check if user exists
            var userExists = await userRepository.isThereUserWithIdAsync(id);
            if (!userExists)
            {
                return "User input is incorrect and user not exists.";
            }
            
            var isActive = await userRepository.isActiveUserWithIdAsync(id);
            return isActive ? "User is active" : "User is not active";
        }

        public async Task<bool> IsUserExists(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.isThereUserWithIdAsync(id);
        }
    }
} 