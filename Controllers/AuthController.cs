using Microsoft.AspNetCore.Mvc;
using WebApplication3.Services;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authservice;
        private readonly IRoleService _roleService;

        public AuthController(IAuthService userManager, IRoleService roleService)
        {
            _authservice = userManager;
            _roleService = roleService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var result = await _authservice.RegisterUserAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _roleService.AssignRoleAsync(model.Email, model.Role);

            return Ok("User registred successfully with role : " + model.Role);

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {

            var tokenResponse = await _authservice.AuthenticateUserAsync(model);

            if (tokenResponse == null)
            {
                return Unauthorized("Invalid Credentials");
            }

            return Ok(tokenResponse);

        }
    }
}
