using Lesson2.Models;

namespace Lesson2.Services
{
    public interface IUserService
    {
        Task<string?> Login(LoginData data);

        Task<User?> Register(User user);
    }
}
