using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rbac.Core.Services.Interfaces;

namespace Rbac.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleProvider roleProvider) : ControllerBase
    {
        private readonly IRoleProvider roleProvider = roleProvider;

        [HttpGet]
        public async Task<IActionResult> GetRolesAsync()
        {
            try
            {
                var roles = await roleProvider.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
