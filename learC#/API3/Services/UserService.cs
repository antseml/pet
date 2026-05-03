using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lesson3.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;


namespace Lesson3.Services
{
    public class UserService : IUserService
    {
        private static List<User> _users = new List<User>{};

        private readonly IGenerate _igenerate;

        public UserService(IGenerate igenerate)
        {
            _igenerate = igenerate;
        }
        
        public string? Login(LoginRequest data)
        {
            var user = _users.FirstOrDefault(u => u.Username == data.Username);
            if(user == null || user.Password != data.Password)return null;
            return _igenerate.GenerateToken(data.Username);
        }

        public User Registration(User user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            return user;
        }
    }
}