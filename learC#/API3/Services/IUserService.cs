using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lesson3.Models;

namespace Lesson3.Services
{
    public interface IUserService
    {
        string? Login(LoginRequest data);

        User Registration(User user);
    }
}