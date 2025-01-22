using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rbac.Core.Data;
using Rbac.Core.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rbac.Core.Services
{
    public class AuthService(IConfiguration configuration, AppDbContext dbContext)
    {
        private readonly IConfiguration configuration = configuration;
        private readonly AppDbContext dbContext = dbContext;
        public string HashPassword(string plainPassword) 
            => BCrypt.Net.BCrypt.HashPassword(plainPassword);

        public bool VerifyPassword(string plainPassword, string hashedPassword) 
            => BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

        public string GenerateJwtToken(User user)
        {
            //assign claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                
            };

            //get user roles
            var roles = dbContext.UserRoles.Where(ur => ur.UserId == user.Id)
                                      .Select(ur => ur.Role.RoleName)
                                      .ToList();
            //add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
