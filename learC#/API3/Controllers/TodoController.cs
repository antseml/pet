using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Lesson3.Models;
using Lesson3.Services;
using Microsoft.Extensions.Logging;

namespace Lesson3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _itodoservice;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService itodoservice, ILogger<TodoController> logger)
        {
            _itodoservice = itodoservice;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation($"Запрос списка задач: {DateTime.Now}");
            return Ok(_itodoservice.GetAll());
        }

        [HttpPost]
        public IActionResult Create(TodoItem todo)
        {
            _logger.LogInformation($"Создание задачи: {DateTime.Now}");
            var created = _itodoservice.Create(todo);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, TodoItem todo)
        {
            _logger.LogInformation($"апдейт задачи с айди = {id} : {DateTime.Now}");
            var updated = _itodoservice.Upgrade(id, todo);
            if(updated == null)return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"удаление задачи с id = {id} : {DateTime.Now}");
            var deleted = _itodoservice.Delete(id);
            return Ok(deleted);
        }
    }
}