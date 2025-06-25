using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> getAllUsersAsync();
        public Task<User?> getUserByIdAsync(int id);
        public Task<bool> isThereUserWithIdAsync(int id);
        public Task<bool> isActiveUserWithIdAsync(int id);
    }
}
