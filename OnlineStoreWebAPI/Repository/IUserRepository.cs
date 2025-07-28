using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IUserRepository
    {
        // Define methods for user-related operations
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        public Task<bool> isThereUserWithId(int userId);
        public Task<bool> isActiveUserWithId(int userId);
    }
}
