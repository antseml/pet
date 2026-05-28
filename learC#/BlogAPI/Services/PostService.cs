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
            return await _context.Posts.AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetByUserId(int userId)
        {
            return await _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Post> Create(Post post, int UserId)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UserId = UserId;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<string?> Update(int id, Post post, int UserId)
        {
            var existing = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if(existing == null) return null;
            if(existing.UserId != UserId) return "permission denied";

            existing.Title = post.Title;
            existing.Content = post.Content;
            await _context.SaveChangesAsync();

            return "successfully";
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
