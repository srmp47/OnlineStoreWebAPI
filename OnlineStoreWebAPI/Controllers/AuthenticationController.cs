using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineStoreWebAPI.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {



        private readonly OnlineStoreDBContext context;
        IConfiguration configuration;
        public AuthenticationController(IConfiguration configuration , OnlineStoreDBContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(UserAuthenticationDTO userDTO)
        {
            var user = ValidateUserCredentials(userDTO.id,userDTO.password);

            if (user == null)
            {
                return Unauthorized();
            }
            Console.WriteLine(user.role);

            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration["Authentication:SecretForKey"])
                );
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256
                );
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("userId", user.userId.ToString()));
            claimsForToken.Add(new Claim("password", user.password));
            claimsForToken.Add(new Claim(ClaimTypes.Role,user.role));

            var jwtSecurityToke = new JwtSecurityToken(
                configuration["Authentication:Issuer"],
                configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                signingCredentials
                );

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToke);
            return Ok(tokenToReturn);
        }

        private User? ValidateUserCredentials(int id,
            string? password)
        {
            
            //if (password == "93589358" && id == 27)
            //    return context.Users.FirstOrDefault(u => u.userId == id);
            //return null;
            var user = context.Users
                .FirstOrDefault(u => u.userId == id && u.password == password);
            return user;
        }


    }
}
