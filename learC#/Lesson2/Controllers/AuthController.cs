using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lesson2.Services;
using Lesson2.Models;
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

        [HttpPost("login")]
        public async Task<IActionResult> Logind(LoginData data)
        {
            var token = await _userservice.login(data);
            if(token == null)
            {
                _logger.LogInformation($"Ошибка аунтификации: {DateTime.Now}");
                return Unauthorized();
            }
            _logger.LogInformation($"Успешная авторизация пользователя({data.Username}) : {DateTime.Now}");
            return Ok(new {token});
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(User user)
        {
            var created = await _userservice.register(user);
            if(created == null)
            {
                _logger.LogInformation($"Попытка повторной регистрации с данными:{user.Username}, {user.Email}. {DateTime.Now}");
                return Conflict("Ползователь с такими данными уже существует");
            }
            _logger.LogInformation($"Создан пользователь({user.Username}) : {DateTime.Now}");
            return Ok(created);
        }
    }
}