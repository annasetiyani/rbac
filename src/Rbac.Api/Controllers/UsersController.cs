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
                // Check if password equals confirm password
                if(request.Password != request.ConfirmPassword)
                    return BadRequest("Passwords do not match.");

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

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var user = await userProvider.GetUserByUsernameAsync(request.Username);

                if (user == null)
                {
                    return Unauthorized("Invalid username.");
                }

                if (!authService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid username.");
                }

                var token = authService.GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
            
            
        }

        // API: Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // JWT token is stateless, so no need to do anything here.
            // In the actual implementation, you may want to remove the token from the client side.
            return Ok("Logged out successfully.");
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserAsync(UserUpdateRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest("Invalid request");

            try
            {
                
                // Check if Email is valid
                if (!ValidatorExtention.IsValidEmail(request.Email))
                    return BadRequest("Invalid Email");
                // Check if Role exists
                if (request.RoleIds == null || request.RoleIds.Count == 0 || request.RoleIds.Any(x => roleProvider.GetRoleByIdAsync(x) == null))
                    return BadRequest("Role does not exist.");
                // Check if user already exists
                var user = await userProvider.GetUserByIdAsync(request.Id);
                if (user == null)
                    return BadRequest("User does not exist.");

                // Update User
                var isUpdated = await userProvider.UpdateUserAsync(request);
                if (!isUpdated)
                    return BadRequest("Failed to update user.");

                var userItem = await userProvider.GetUserByIdAsync(request.Id);

                return Ok(userItem);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
            
        }

        // API: Delete User
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Id");

            try
            {
                // Check if user exists
                var user = await userProvider.GetUserByIdAsync(id);
                if (user == null)
                    return BadRequest("User does not exist.");

                // Delete User
                var isDeleted = await userProvider.DeleteUserAsync(id);
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
            
        }

        [HttpPost("activate/{id}")]
        public async Task<IActionResult> ActivateUserAsync(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Id");
            try
            {
                // Check if user exists
                var user = await userProvider.GetUserByIdAsync(id);
                if (user == null)
                    return BadRequest("User does not exist.");

                // Activate User
                bool isActivated = await userProvider.UpdateActiveAsync(id, false);
                if (!isActivated)
                    return BadRequest("Failed to activate user.");

                user.IsActive = true;

                return Ok(user);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }

        }
        
        [HttpPost("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUserAsync(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Id");
            try
            {
                // Check if user exists
                var user = await userProvider.GetUserByIdAsync(id);
                if (user == null)
                    return BadRequest("User does not exist.");

                // Deactivate User
                var isDeactivated = await userProvider.UpdateActiveAsync(id, false);
                if (!isDeactivated)
                    return BadRequest("Failed to deactivate user.");

                user.IsActive = false;
                return Ok(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }

            
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(UserForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");
            // Check if Password equals Confirm Password
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");
            try
            {
                // Check if user exists
                var user = await userProvider.GetUserByUsernameAsync(request.Username);
                if (user == null)
                    return BadRequest("User does not exist.");
                // hash new password
                var hashedPassword = authService.HashPassword(request.NewPassword);
                // Update password
                var isUpdated = await userProvider.UpdatePasswordAsync(user.Id, hashedPassword);
                if(!isUpdated)
                    return BadRequest("Failed to update password.");

                return Ok("Password reset successful.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal Server Error");
            }

        }
    }
}
