using Lesson2.Models;

namespace Lesson2.Services
{
    public interface ICommentService
    {
        Task<Comment?> Create(Comment comment, int UserId, int PostId);
        Task<string?> Delete(int id, int UserId);
    }
}