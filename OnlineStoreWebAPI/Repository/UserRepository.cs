using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly OnlineStoreDBContext _context;
        public UserRepository(OnlineStoreDBContext _context)
        {
            this._context = _context;
        }
        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.userId == userId);
            if (user == null)
            {
                return false; // User not found
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var result =  _context.Users.GetAsyncEnumerator;
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.userId == userId);
            return user;
        }

        // TODO search about these methodes and their implemention
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Attach(user);  
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
        }

        public async Task<bool> isActiveUserWithId(int userId)
        {
            var isActive = await _context.Users.AnyAsync(u => u.userId == userId && u.isActive);
            return isActive;
        }

        public async Task<bool> isThereUserWithId(int userId)
        {
            var isThere = await _context.Users.AnyAsync(u => u.userId == userId);
            return isThere;
        }
    }
}
