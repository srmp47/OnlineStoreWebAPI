using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        public UserService(IUserRepository userRepository, IMapper inputMapper)
        {
            this.userRepository = userRepository;
            this.mapper = inputMapper;
        }

        public async Task<bool>  activateUserByIdAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null) return false;
            user.isActive = true;
            await userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<User> createNewUserAsync(User user)
        {
            await userRepository.CreateUserAsync(user);
            return user;
        }

        public async  Task<bool> deActivateUserByUserIdAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null) return false;
            user.isActive = false;
            await userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> deleteUserByIdAsync(int id)
        {
            var isDeletedSuccessfully = await userRepository.DeleteUserAsync(id);
            return isDeletedSuccessfully;
        }

        public async Task<IEnumerable<User>> getAllUsersAsync(PaginationParameters paginationParameters)
        {
            var allUsers = await userRepository.GetAllUsersAsync();
            return allUsers;

        }

        public async Task<User?> getUserByIdAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            return user;
        }

        public async Task<bool> isActiveUserWithIdAsync(int id)
        {
            var isActive = await userRepository.isActiveUserWithId(id);
            return isActive;
        }

        public async Task<bool> isThereUserWithIdAsync(int id)
        {
            var result = await userRepository.isThereUserWithId(id);
            return result;
        }

        
        // TODO implement partial update
        //TODO review this method:
        public async Task<User> updateUserAsync(int id, UserUpdateDTO userDTO)
        {
            var user = mapper.Map<User>(userDTO);
            user.userId = id;
            await userRepository.UpdateUserAsync(user);
            return user;
        }
    }
}
