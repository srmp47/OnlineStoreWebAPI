using HotChocolate;
using HotChocolate.Types;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;
using OnlineStoreWebAPI.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using HotChocolate.Authorization;
using System.Security.Claims;
namespace OnlineStoreWebAPI.GraphQL
{
    public class UserMutation
    {

        //TODO: check validation of Data Annotations
        //(now you can create user with this pass:"a"=>minLength!?)
        [Authorize(Roles = new[] { "Admin" })]
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
        [Authorize(Roles = new[] { "Admin" })]
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
        [Authorize(Roles = new[] { "Admin" })]
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
        
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<User> CreateUser(UserWithoutIsActiveDTO inputUser, [Service] UserRepository userRepository, [Service] AutoMapper.IMapper mapper)
        {
            var user = mapper.Map<User>(inputUser);
            return await userRepository.createNewUserAsync(user);
        }
        [Authorize]
        public async Task<User> UpdateUser(ClaimsPrincipal claims, UserUpdateDTO inputUser, [Service] UserRepository userRepository)
        {
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            if (await userRepository.isThereUserWithIdAsync(userId))
            {
                throw new GraphQLException($"User with ID {userId} not found.");
            }
            
            return await userRepository.updateUserAsync(userId, inputUser);
        }
        public async Task<User> signUp(UserWithoutIsActiveDTO inputUser, [Service] UserRepository userRepository, [Service] AutoMapper.IMapper mapper)
        {
            if (inputUser == null) throw new GraphQLException("input is null");
            var user = mapper.Map<User>(inputUser);
            return await userRepository.createNewUserAsync(user);
        }

    }
} 