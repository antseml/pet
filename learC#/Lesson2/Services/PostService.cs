using Lesson2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson2.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;
        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetAll() 
        {
            return await _context.Posts.AsNoTracking().Include(p => p.User).ToListAsync();
        }

        public async Task<Post?> GetById(int id)
        {
            var post = await _context.Posts.AsNoTracking()
                .Include(p => p.User)
                .ThenInclude(u => u.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
            return post;
        }

        public async Task<Post> Create(Post post, int UserId)
        {
            post.CreatedAt = DateTime.Now;
            post.UserId = UserId;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<string?> Delete(int id, int UserId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if(post == null)return "Not Found";
            if(post.UserId != UserId)return "permission denied"; 
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return "successfully";
        }
    }
}