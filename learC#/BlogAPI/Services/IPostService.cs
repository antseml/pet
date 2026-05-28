using Lesson2.Models;

namespace Lesson2.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAll();
        Task<Post?> GetById(int id);
        Task<Post> Create(Post post, int UserId);
        Task<string?> Update(int id, Post post, int UserId);
        Task<string?> Delete(int id, int UserId);
        Task<List<Post>> GetByUserId(int userId);
    }
}
