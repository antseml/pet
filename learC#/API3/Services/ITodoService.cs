using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lesson3.Models;

namespace Lesson3.Services
{
    public interface ITodoService
    {
        List<TodoItem> GetAll();
        TodoItem Create(TodoItem _todo);
        TodoItem Upgrade(int id, TodoItem _todo);
        TodoItem Delete(int id);
    }
}