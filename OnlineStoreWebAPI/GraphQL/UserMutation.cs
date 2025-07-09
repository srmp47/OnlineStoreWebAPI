using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using OnlineStoreWebAPI.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
namespace OnlineStoreWebAPI.GraphQL
{
    public class UserMutation
    {

        //TODO: check validation of Data Annotations
        //(now you can create user with this pass:"a"=>minLength!?)

        public async Task<string> ActivateUser(int id, [Service] UserRepository userRepository)
        {
            var userExists = await userRepository.isThereUserWithIdAsync(id);
            if (!userExists)
            {
                return "User input is incorrect and user not exists.";
            }
            
            var result = await userRepository.activateUserByIdAsync(id);
            return result ? "User activated successfully" : "Failed to activate user";
        }

        public async Task<string> DeactivateUser(int id, [Service] UserRepository userRepository)
        {
            var userExists = await userRepository.isThereUserWithIdAsync(id);
            if (!userExists)
            {
                return "User input is incorrect and user not exists.";
            }
            
            var result = await userRepository.deActivateUserByUserIdAsync(id);
            return result ? "User deactivated successfully" : "Failed to deactivate user";
        }

        public async Task<string> DeleteUser(int id, [Service] UserRepository userRepository)
        {
            var userExists = await userRepository.isThereUserWithIdAsync(id);
            if (!userExists)
            {
                return "User input is incorrect and user not exists.";
            }
            
            var deletedUser = await userRepository.deleteUserByIdAsync(id);
            return deletedUser != null ? $"User {deletedUser.firstName} deleted successfully" : "Failed to delete user";
        }

        public async Task<string> CreateUser(UserWithoutIsActiveDTO inputUser, [Service] UserRepository userRepository, [Service] AutoMapper.IMapper mapper)
        {
            var user = mapper.Map<User>(inputUser);
            var createdUser = await userRepository.createNewUserAsync(user);
            return $"User {createdUser.firstName} created successfully with ID: {createdUser.userId}";
        }

        public async Task<string> UpdateUser(int id, UserUpdateDTO inputUser, [Service] UserRepository userRepository)
        {
            var userExists = await userRepository.isThereUserWithIdAsync(id);
            if (!userExists)
            {
                return "User input is incorrect and user not exists.";
            }
            
            var updatedUser = await userRepository.updateUserAsync(id, inputUser);
            return $"User {updatedUser.firstName} updated successfully";
        }
    }
} 