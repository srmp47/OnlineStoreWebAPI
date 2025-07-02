using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly OnlineStoreDBContext context;
        private readonly IMapper mapper;
        public UserRepository(OnlineStoreDBContext inputContext, IMapper inputMapper)
        {
            this.context = inputContext;
            this.mapper = inputMapper;
        }

        public async Task<bool>  activateUserByIdAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if(user == null) return false;
            user.isActive = true;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<User> createNewUserAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async  Task<bool> deActivateUserByUserIdAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if(user == null) return false;
            user.isActive = false;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<User> deleteUserByIdAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.userId == id);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return user;
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
            return user.isActive;
        }

        public async Task<bool> isThereUserWithIdAsync(int id)
        {
            return await context.Users.AnyAsync(u => u.userId == id);
        }

        
        public async Task<User> updateUserAsync(int id, UserUpdateDTO user)
        {
            var currentUser = await context.Users.FirstOrDefaultAsync(u => u.userId == id);
            if(user.firstName != null)currentUser.firstName = user.firstName;
            if (user.lastName != null) currentUser.email = user.email;
            if (user.password != null) currentUser.password = user.password;
            if (user.email != null) currentUser.email = user.email;
            if (user.address != null) currentUser.address = user.address;
            context.Update(currentUser);
            await context.SaveChangesAsync();
            return currentUser;
        }
    }
}
