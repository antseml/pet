using Lesson2.Models;
using Lesson2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lesson2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userservice, ILogger<AuthController> logger)
        {
            _userservice = userservice;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                HashPassword = request.Password
            };

            var created = await _userservice.Register(user);
            if (created == null)
            {
                _logger.LogInformation("Registration failed: user already exists {Username}/{Email}", request.Username, request.Email);
                return Conflict(new { message = "User with this username or email already exists." });
            }

            _logger.LogInformation("User registered {Username}", request.Username);
            return Created(string.Empty, new { id = created.Id, created.Username, created.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest data)
        {
            var loginData = new LoginData
            {
                Username = data.Username,
                Password = data.Password
            };

            var token = await _userservice.Login(loginData);
            if (token == null)
            {
                _logger.LogInformation("Invalid login attempt for {Username}", data.Username);
                return Unauthorized(new { message = "Invalid username or password." });
            }

            _logger.LogInformation("Successful login {Username}", data.Username);
            return Ok(new { token });
        }
    }
}
