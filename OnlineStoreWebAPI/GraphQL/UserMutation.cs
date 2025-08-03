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
        public async Task<User> ActivateUser(int id, [Service] UserService userService)
        {
            var user = await userService.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            var result = await userService.activateUserByIdAsync(id);
            if (!result)
            {
                throw new GraphQLException("Failed to activate user.");
            }
            
            return await userService.getUserByIdAsync(id);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<User> DeactivateUser(int id, [Service] UserService userService)
        {
            var user = await userService.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            var result = await userService.deActivateUserByUserIdAsync(id);
            if (!result)
            {
                throw new GraphQLException("Failed to deactivate user.");
            }
            
            return await userService.getUserByIdAsync(id);
        }
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> DeleteUser(int id, [Service] UserService userService)
        {
            var user = await userService.getUserByIdAsync(id);
            if (user == null)
            {
                throw new GraphQLException($"User with ID {id} not found.");
            }
            
            var isDeletedSuccessfully = await userService.deleteUserByIdAsync(id);
            if (isDeletedSuccessfully == false)
            {
                throw new GraphQLException("Failed to delete user.");
            }
            
            return isDeletedSuccessfully;
        }
        
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<User> CreateUser(UserWithoutIsActiveDTO inputUser, [Service] UserService userService, [Service] AutoMapper.IMapper mapper)
        {
            var user = mapper.Map<User>(inputUser);
            return await userService.createNewUserAsync(user);
        }
        [Authorize]
        public async Task<User> UpdateUser(ClaimsPrincipal claims, UserUpdateDTO inputUser, [Service] UserService userService)
        {
            int userId = Convert.ToInt32(claims.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
            if (!await userService.isThereUserWithIdAsync(userId))
            {
                throw new GraphQLException($"User with ID {userId} not found.");
            }
            var context = new ValidationContext(inputUser);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(inputUser, context, results, validateAllProperties: true))
            {
                var messages = results.Select(r => r.ErrorMessage);
                throw new GraphQLException(string.Join(" | ", messages));
            }

            return await userService.updateUserAsync(userId, inputUser);
        }
        public async Task<User> signUp(UserWithoutIsActiveDTO inputUser, [Service] UserService userService, [Service] AutoMapper.IMapper mapper)
        {
            if (inputUser == null)
                throw new GraphQLException("Input is null");
            var context = new ValidationContext(inputUser);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(inputUser, context, results, validateAllProperties: true))
            {
                var messages = results.Select(r => r.ErrorMessage);
                throw new GraphQLException(string.Join(" | ", messages));
            }

            var user = mapper.Map<User>(inputUser);
            return await userService.createNewUserAsync(user);
        }

    }
} 