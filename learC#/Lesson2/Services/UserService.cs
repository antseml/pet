using Lesson2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson2.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ITokenGenerate _token;
        public UserService(AppDbContext context, ITokenGenerate token)
        {
            _context = context;
            _token = token;
        }

        public async Task<string?> login(LoginData data)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == data.Username || u.Email == data.Username);
            if(user == null || BCrypt.Net.BCrypt.Verify(data.Password, user.HashPassword) == false)return null;
            return _token.GenerateToken(user.Username, user.Id);
        }

        public async Task<User?> register(User user)
        {
            var _user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);
            if(_user != null)return null;
            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(user.HashPassword);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}