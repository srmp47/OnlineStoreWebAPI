using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/User")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<User>>> getAllUsers()
        {
            var Users = await userRepository.getAllUsersAsync();
            if (Users == null) return NoContent();
            return Ok(Users);

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
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> activateUserById(int id)
        {
            var success = await userRepository.activateUserByIdAsync(id);
            if(success) return Ok();
            else return NotFound("User Not Found!");
        }
        [HttpPost("AddUser")]
        public async Task<IActionResult> createNewUser([FromBody] UserWithoutIsActiveDTO inputUser)
        {
            if (inputUser == null) return BadRequest("User is null");
            var user = mapper.Map<User>(inputUser);
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                await userRepository.createNewUserAsync(user);
                return Ok("User Added successfully!");
            }
        }

    }
}
