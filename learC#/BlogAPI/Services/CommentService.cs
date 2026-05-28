using Lesson2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson2.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> Create(Comment comment, int UserId, int PostId)
        {
            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == PostId);
            if(post == null)return null;
            comment.CreatedAt = DateTime.UtcNow;
            comment.UserId = UserId;
            comment.PostId = PostId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetByPostId(int postId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<string?> Delete(int id, int UserId){
            var comment = await _context.Comments.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if(comment == null)return "Not Found";
            if(comment.UserId != UserId)return "permission denied";
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return "successfully";
        }
    }
}
