using Lesson2.Models;

namespace Lesson2.Services
{
    public interface IUserService
    {
        Task<string?> login(LoginData data);

        Task<User?> register(User user);
    }
}