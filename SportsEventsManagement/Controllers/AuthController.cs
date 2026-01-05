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
        public async Task<IActionResult> Register(Utilisateur utilisateur)
        {
            var user = await _authService.Register(utilisateur);
            if (user == null)
            {
                return BadRequest("Email already exists.");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // [CHANGE] Call the service which now returns a Token (string)
            var token = await _authService.Login(request.Email, request.Password);

            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // [CHANGE] Return the token inside a JSON object
            return Ok(new { Token = token });
        }

        // [NEW] Helper class to read the JSON body sent by the frontend
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}