using Microsoft.AspNetCore.Mvc;
using SportsEventsManagement.Models;
using SportsEventsManagement.Services;

namespace SportsEventsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var newUser = new Utilisateur
            {
                Email = request.Email,

                // Map English request to French model properties
                MotDePasse = request.Password,
                Nom = "New User",   // Default value (since RegisterRequest doesn't have Name yet)
                Telephone = "",     // Default value

                Role = request.Role
            };

            var user = await _authService.Register(newUser);

            if (user == null)
            {
                return BadRequest("Email already exists.");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.Login(request.Email, request.Password);

            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { Token = token });
        }

        // --- DTO CLASSES ---

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // [NEW] Helper class for Registration
        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = "User"; // Default to "User" if not sent
        }
    }
}