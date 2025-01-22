using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rbac.Core.Data.Models;
using Rbac.Core.Services;
using Rbac.Core.Services.Extentions;
using Rbac.Core.Services.Interfaces;

namespace Rbac.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> logger
        , IUserProvider userProvider
        ,   IRoleProvider roleProvider
        , AuthService authService) 
        : ControllerBase
    {
        private readonly ILogger<UsersController> logger = logger;
        private readonly IUserProvider userProvider = userProvider;
        private readonly AuthService authService = authService;
        private readonly IRoleProvider roleProvider = roleProvider;

        // API: Get All Users
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
                var users = await userProvider.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
            
        }

        // API: Get User By Id
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByIdAsync(Guid id)
        {
            try
            {
                if(!ModelState.IsValid || id == Guid.Empty)
                    return BadRequest("Invalid Id");
                var user = await userProvider.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }

        }

        // API: Create User
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateRequest request)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest("Invalid request");
                // Check if Email is valid
                if (!ValidatorExtention.IsValidEmail(request.Email))
                    return BadRequest("Invalid Email");
                // Check if Role exists
                if (request.RoleId == Guid.Empty || await roleProvider.GetRoleByIdAsync(request.RoleId) == null)
                    return BadRequest("Role does not exist.");
                // Check if user already exists
                if (await userProvider.GetUserByUsernameAsync(request.Username)!=null)
                    return BadRequest("Username already exists.");
                
                var hashedPassword = authService.HashPassword(request.Password);

                // Create User
                var userId = await userProvider.CreateUserAsync(request, hashedPassword);

                return Ok(await userProvider.GetUserByIdAsync(userId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
            
        }

    }
}
