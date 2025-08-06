using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestProject.Integration
{
    public static class TestAuthHelper
    {
        public static readonly string TestSecretKey = "TestSecretKeyForIntegrationTests12345678901234567890";
        public static readonly string TestIssuer = "TestIssuer";
        public static readonly string TestAudience = "TestAudience";

        public static string GenerateTestToken(string role = "Admin", string userId = "2")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(TestSecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, "testuser@example.com"),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Email, "testuser@example.com"),
                    new Claim("userId", userId)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = TestIssuer,
                Audience = TestAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GetTestAuthorizationHeader(string role = "Admin", string userId = "2")
        {
            var token = GenerateTestToken(role, userId);
            return $"Bearer {token}";
        }
    }
}