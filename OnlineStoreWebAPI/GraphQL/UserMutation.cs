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

        public async Task<User> ActivateUser(int id, [Service] UserRepository userRepository)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            var result = await userRepository.activateUserByIdAsync(id);
            if (!result)
            {
                throw new GraphQLException("Failed to activate user.");
            }
            
            return await userRepository.getUserByIdAsync(id);
        }

        public async Task<User> DeactivateUser(int id, [Service] UserRepository userRepository)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            var result = await userRepository.deActivateUserByUserIdAsync(id);
            if (!result)
            {
                throw new GraphQLException("Failed to deactivate user.");
            }
            
            return await userRepository.getUserByIdAsync(id);
        }

        public async Task<User> DeleteUser(int id, [Service] UserRepository userRepository)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            var deletedUser = await userRepository.deleteUserByIdAsync(id);
            if (deletedUser == null)
            {
                throw new GraphQLException("Failed to delete user.");
            }
            
            return deletedUser;
        }

        public async Task<User> CreateUser(UserWithoutIsActiveDTO inputUser, [Service] UserRepository userRepository, [Service] AutoMapper.IMapper mapper)
        {
            var user = mapper.Map<User>(inputUser);
            return await userRepository.createNewUserAsync(user);
        }

        public async Task<User> UpdateUser(int id, UserUpdateDTO inputUser, [Service] UserRepository userRepository)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            return await userRepository.updateUserAsync(id, inputUser);
        }
    }
} 