using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using OnlineStoreWebAPI.Repository;
using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/User")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserService userRepository;
        public UserController(IMapper mapper, IUserService userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> getAllUsers
            ([FromQuery] PaginationParameters paginationParameters)
        {

            var result = await userRepository.getAllUsersAsync(paginationParameters);
            if (result == null) return NoContent();
            return Ok(result);


        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
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
        // TODO: implement patch document in patch methodes.
        [HttpPatch("{id}/Activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> activateUserById(int id)
        {
            var success = await userRepository.activateUserByIdAsync(id);
            if (success) return Ok("User Activated Successfully");
            else return NotFound("User Not Found!");
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteUserById(int id)
        {
            var isValidId = await userRepository.isThereUserWithIdAsync(id);
            if (!isValidId) return NotFound("User not exist");
            var result = await userRepository.deleteUserByIdAsync(id);
            return Ok(result);

        }
        // TODO correct this method. if user enters an invalid email address,
        // email field will be empty
        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/DeActivate")]
        public async Task<IActionResult> deActivateUserById(int id)
        {
            var success = await userRepository.deActivateUserByUserIdAsync(id);
            if (success) return Ok("User De Activated Successfully");
            else return NotFound("User Not Found!");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/isActive")]
        public async Task<IActionResult> isActiveUserById(int id)
        {
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null) return NotFound("User Not Found");
            var isActive = await userRepository.isActiveUserWithIdAsync(id);
            if (isActive) return Ok("User is active");
            else return Ok("User is not active");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/isThere")]
        public async Task<IActionResult> isThereUserWithId(int id)
        {
            if (await userRepository.isThereUserWithIdAsync(id)) return Ok("There is");
            else return Ok("There is not");

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/Update")]
        public async Task<IActionResult> updateUserById(int id, [FromBody] UserUpdateDTO userDTO)
        {

            if (userDTO == null) return BadRequest("User is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            var isValidId = await userRepository.isThereUserWithIdAsync(id);
            if (!isValidId) return BadRequest("User Not Found");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var result = await userRepository.updateUserAsync(id, userDTO);
                return Ok(result);
            }
        }
        //TODO could not test this method
        //[Authorize]
        //[HttpPatch]
        //public async Task<IActionResult> PatchUser( [FromBody] JsonPatchDocument<UserUpdateDTO> patchDoc)
        //{
        //    if (patchDoc == null)
        //        return BadRequest("Patch document is null");
        //    var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
        //    int id = Convert.ToInt32(claimId.Value);
        //    var user = await userRepository.getUserByIdAsync(id);

        //    var userToPatch = mapper.Map<UserUpdateDTO>(user);

        //    patchDoc.ApplyTo(userToPatch); 

        //    if (!TryValidateModel(userToPatch))
        //        return BadRequest(ModelState);

        //    mapper.Map(userToPatch, user);
        //    await userRepository.partialUpdateUser(user);

        //    return NoContent();
        //}


        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> updateUser([FromBody] UserUpdateDTO userDTO)
        {
            if (userDTO == null) return BadRequest("User is null");
            if (!ModelState.IsValid) return BadRequest("Enter Valid Information");
            else
            {
                var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
                int id = Convert.ToInt32(claimId.Value);
                var result = await userRepository.updateUserAsync(id, userDTO);
                return Ok(result);
            }



        }
        [Authorize]

        [HttpGet("show my information")]
        public async Task<IActionResult> getMyInformation()
        {
            var claimId = User.Claims.FirstOrDefault(u => u.Type == "userId");
            int id = Convert.ToInt32(claimId.Value);
            var user = await userRepository.getUserByIdAsync(id);
            if (user == null) return NotFound("User Not Found");
            return Ok(user);
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> signup([FromBody] UserWithoutIsActiveDTO inputUser)
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
    }
    }

