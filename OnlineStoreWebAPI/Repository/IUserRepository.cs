using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> getAllUsersAsync();
        public Task<User?> getUserByIdAsync(int id);
        public Task<bool> isThereUserWithIdAsync(int id);
        public Task<bool> isActiveUserWithIdAsync(int id);
        public Task<User> createNewUserAsync(User user);
        public Task<User> activateUserByUserIdAsync(int id);
        public Task<User> deActivateUserByUserIdAsync(int id);
        public Task<User> updateUserAsync(User user);


    }
}
