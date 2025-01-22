using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rbac.Core.Services
{
    public class AuthService(IConfiguration configuration)
    {
        private readonly IConfiguration configuration = configuration;

        public string HashPassword(string plainPassword) 
            => BCrypt.Net.BCrypt.HashPassword(plainPassword);

        public bool VerifyPassword(string plainPassword, string hashedPassword) 
            => BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

        public string GenerateJwtToken(string username, int roleId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, roleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
