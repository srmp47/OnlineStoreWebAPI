using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly OnlineStoreDBContext context;
        public UserRepository(OnlineStoreDBContext inputContext)
        {
            this.context = inputContext;
        }

        public Task<User> activateUserByUserIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> createNewUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> deActivateUserByUserIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> getAllUsersAsync()
        {
            return await context.Users.OrderBy(u => u.userId).ToListAsync() ;
        }

        public async Task<User?> getUserByIdAsync(int id)
        {
            return await context.Users.OrderBy(u => u.userId).Where(u => u.userId == id).FirstOrDefaultAsync();
        }

        public async Task<bool> isActiveUserWithIdAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if (user == null) return false;
            else return user.isActive;
        }

        public async Task<bool> isThereUserWithIdAsync(int id)
        {
            return await context.Users.AnyAsync(u => u.userId == id);
        }

        public Task<User> updateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
