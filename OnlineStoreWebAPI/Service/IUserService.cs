using Microsoft.AspNetCore.JsonPatch;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> getAllUsersAsync(PaginationParameters paginationParameters);
        public Task<User?> getUserByIdAsync(int id);
        public Task<bool> isThereUserWithIdAsync(int id);
        public Task<bool> isActiveUserWithIdAsync(int id);
        public Task<User> createNewUserAsync(User user);
        public Task<bool> activateUserByIdAsync(int id);
        public Task<bool> deActivateUserByUserIdAsync(int id);
        public Task<User> updateUserAsync(int id,UserUpdateDTO userDTO);
        public Task<bool> deleteUserByIdAsync(int id);
        public  Task partialUpdateUser(User user);

    }
}
