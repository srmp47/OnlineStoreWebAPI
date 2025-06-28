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
        [HttpPatch("{id}/Activate")]
        public async Task<IActionResult> activateUserById(int id)
        {
            var success = await userRepository.activateUserByIdAsync(id);
            if(success) return Ok("User Activated Successfully");
            else return NotFound("User Not Found!");
        }
        [HttpGet("{id}/Delete")]
        public async  Task<IActionResult> deleteUserById(int id){
            var isValidId = await userRepository.isThereUserWithIdAsync(id);
            if (!isValidId) return NotFound("User not exist");
            var user = await userRepository.deleteUserByIdAsync(id);
            return Ok(user);
            
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
        // this method does not work correctly!!!
        // see update method in repository
        // how to implement transforing data from userDTO to database?
        //I could not do it by mapping!
        [HttpPost("{id}/Update")]
        public async Task<IActionResult> updateUser(int id,[FromBody] UserWithoutIsActiveDTO inputUser)
        {
            if (inputUser == null) return BadRequest("User is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var isValidId = await userRepository.isThereUserWithIdAsync(id);
            if (!isValidId) return BadRequest("User Not Found");
            var user = mapper.Map<User>(inputUser);
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var result = await userRepository.updateUserAsync(id,user);
                return Ok(result);
            }
        }



    }
}
