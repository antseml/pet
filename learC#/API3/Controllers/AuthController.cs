using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson3.Models;
using Lesson3.Services;
using Microsoft.Extensions.Logging;

namespace Lesson3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _iuserservice;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService iuserservice, ILogger<AuthController> logger)
        {
            _iuserservice = iuserservice;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var token = _iuserservice.Login(request);
            if (token == null){
                _logger.LogInformation($"не успешная попытка авторизации по логину: {request.Username}, {DateTime.Now}");
                return Unauthorized();
            }
            _logger.LogInformation($"Успешная авторизация по логину: {request.Username}, {DateTime.Now}");
            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            _logger.LogInformation($"Создание пользователя с логином = {user.Username} : {DateTime.Now}");
            var created = _iuserservice.Registration(user);
            return Ok(created);
        }
    }
}