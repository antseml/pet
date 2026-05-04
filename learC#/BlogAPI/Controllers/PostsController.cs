using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Lesson2.Services;
using Lesson2.Models;
using Microsoft.Extensions.Logging;

namespace Lesson2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postservice;
        private readonly ICommentService _commentservice;
        private readonly ILogger <PostsController> _logger;

        public PostsController(IPostService postservice, ICommentService commentservice, ILogger <PostsController> logger)
        {
            _commentservice = commentservice;
            _postservice = postservice;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Получение списка постов: {DateTime.Now}");
            var posts = await _postservice.GetAll();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _postservice.GetById(id);
            if(post == null)
            {
                _logger.LogInformation($"Попытка получения несуществующего поста(id = {id}) : {DateTime.Now}");
                return NotFound();
            }
            _logger.LogInformation($"Поста с id = {id} Успешно получен : {DateTime.Now}");
            return Ok(post);
        }

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var created = await _postservice.Create(post, userId);
            _logger.LogInformation($"Создание поста от пользователя(id = {userId}) : {DateTime.Now}");
            return Ok(created);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var deleted = await _postservice.Delete(id, userId);
            if(deleted == "Not Found")
            {
                _logger.LogInformation($"Попытка удаления несуществующего поста : {userId}, {DateTime.Now}");
                return NotFound();
            }
            if(deleted == "permission denied")
            {
                _logger.LogInformation($"Попытка удаления не своего поста : {userId}, {DateTime.Now}");
                return Conflict("Вы не являетесь создателем поста");
            }
            _logger.LogInformation($"Пост с id = {id} удален : {userId}, {DateTime.Now}");
            return Ok();
        }

        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment(Comment comment, int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var created = await _commentservice.Create(comment, userId, id);
            if(created == null)
            {
                _logger.LogInformation($"Попытка создания комментария под несуществующим постом от пользователя с id = {userId} : {DateTime.Now}");
                return Conflict("Данного поста не существует");
            }
            _logger.LogInformation($"Пользователь({userId}) создал комментарий({created.Id}) под постом({id}) : {DateTime.Now}");
            return Ok(created);
        }

        [HttpDelete("api/comments/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var deleted = await _commentservice.Delete(id, userId);
            if(deleted == "Not Found")
            {
                _logger.LogInformation($"Попытка удаления несуществующего комментария : {userId}, {DateTime.Now}");
                return NotFound();
            }
            if(deleted == "permission denied")
            {
                _logger.LogInformation($"Попытка удаления не своего комментария : {userId}, {DateTime.Now}");
                return Conflict("Вы не являетесь создателем комментария");
            }
            _logger.LogInformation($"Пост с id = {id} удален : {userId}, {DateTime.Now}");
            return Ok();
        }
    }
}