using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        public UserController(IMapper mapper, IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> getAllUsers
            ([FromQuery] PaginationParameters paginationParameters)
        {
            var result = await userRepository.getAllUsersAsync(paginationParameters);
            if (result == null) return NoContent();
            return Ok(result);
           

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getUserById(int id, bool showOrders)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null) return NotFound();
            if (showOrders) return Ok(user);
            else
            {
                var userWithoutOrders = mapper.Map<UserWithoutOrdersDTO>(user);
                return Ok(userWithoutOrders);
            }
        }
        [HttpPatch("{id}/Activate")]
        public async Task<IActionResult> activateUserById(int id)
        {
            var success = await userRepository.activateUserByIdAsync(id);
            if(success) return Ok("User Activated Successfully");
            else return NotFound("User Not Found!");
        }
        [HttpDelete("{id}")]
        public async  Task<IActionResult> deleteUserById(int id){
            var isValidId = await userRepository.isThereUserWithIdAsync(id);
            if (!isValidId) return NotFound("User not exist");
            var result = await userRepository.deleteUserByIdAsync(id);
            return Ok(result);
            
        }
        [HttpPost("AddUser")]
        public async Task<IActionResult> createNewUser([FromBody] UserWithoutIsActiveDTO inputUser)
        {
            if (inputUser == null) return BadRequest("User is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var user = mapper.Map<User>(inputUser);
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                await userRepository.createNewUserAsync(user);
                return Ok("User Added successfully!");
            }
        }
        [HttpPatch("{id}/DeActivate")]
        public async Task<IActionResult> deActivateUserById(int id)
        {
            var success = await userRepository.deActivateUserByUserIdAsync(id);
            if (success) return Ok("User De Activated Successfully");
            else return NotFound("User Not Found!");
        }
        [HttpGet("{id}/isActive")]
        public async Task<IActionResult> isActiveUserById(int id)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null) return NotFound("User Not Found");
            var isActive = await userRepository.isActiveUserWithIdAsync(id);
            if (isActive) return Ok("User is active");
            else return Ok("User is not active");
        }
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereUserWithId(int id)
        {
            if ( await userRepository.isThereUserWithIdAsync(id)) return Ok("There is");
            else return Ok("There is not");
            
        }
        [HttpPost("{id}/Update")]
        public async Task<IActionResult> updateUser(int id,[FromBody] UserUpdateDTO user)
        {
            if (user == null) return BadRequest("User is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var isValidId = await userRepository.isThereUserWithIdAsync(id);
            if (!isValidId) return BadRequest("User Not Found");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var result = await userRepository.updateUserAsync(id,user);
                return Ok(result);
            }
        }



    }
}
