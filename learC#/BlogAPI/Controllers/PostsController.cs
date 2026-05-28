using Lesson2.Models;
using Lesson2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lesson2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postservice;
        private readonly ICommentService _commentservice;

        public PostsController(
            IPostService postservice,
            ICommentService commentservice)
        {
            _commentservice = commentservice;
            _postservice = postservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postservice.GetAll();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _postservice.GetById(id);
            if (post == null)
            {
                return NotFound(new { message = $"Post {id} not found." });
            }

            return Ok(post);
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var post = await _postservice.GetById(id);
            if (post == null)
            {
                return NotFound(new { message = $"Post {id} not found." });
            }

            var comments = await _commentservice.GetByPostId(id);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            var post = new Post
            {
                Title = request.Title.Trim(),
                Content = request.Content.Trim()
            };

            var created = await _postservice.Create(post, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
        {
            var toUpdate = new Post
            {
                Title = request.Title.Trim(),
                Content = request.Content.Trim()
            };

            var result = await _postservice.Update(id, toUpdate, GetCurrentUserId());
            if (result == null)
            {
                return NotFound(new { message = $"Post {id} not found." });
            }

            if (result == "permission denied")
            {
                return Forbid();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _postservice.Delete(id, GetCurrentUserId());
            if (deleted == "Not Found")
            {
                return NotFound(new { message = $"Post {id} not found." });
            }
            if (deleted == "permission denied")
            {
                return Forbid();
            }

            return NoContent();
        }

        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment(int id, [FromBody] CreateCommentRequest request)
        {
            var comment = new Comment
            {
                Text = request.Text.Trim()
            };

            var created = await _commentservice.Create(comment, GetCurrentUserId(), id);
            if (created == null)
            {
                return NotFound(new { message = $"Post {id} not found." });
            }

            return Created($"/api/posts/{id}/comments", created);
        }

        [HttpDelete("~/api/comments/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var deleted = await _commentservice.Delete(id, GetCurrentUserId());
            if (deleted == "Not Found")
            {
                return NotFound(new { message = $"Comment {id} not found." });
            }
            if (deleted == "permission denied")
            {
                return Forbid();
            }

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimValue, out var userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Invalid user identifier in JWT token.");
        }
    }
}
