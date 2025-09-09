using HD.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HD.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceAuthentication _authService;

        public AuthController(IServiceAuthentication authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            try
            {
                var result = _authService.Authenticate(login.Username, login.Password);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
