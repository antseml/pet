using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lesson3.Models;

namespace Lesson3.Services
{
    public class TodoService : ITodoService
    {
        private static List<TodoItem> _todoitems = new List<TodoItem>{};

        public List<TodoItem> GetAll() => _todoitems;

        public TodoItem Create(TodoItem _todo)
        {
            _todo.Id = _todoitems.Any() ? _todoitems.Max(td => td.Id) + 1 : 1;
            _todo.CreatedAt = DateTime.Now;
            _todoitems.Add(_todo);
            return _todo;   
        }

        public TodoItem Upgrade(int id, TodoItem _todo)
        {
            var todo = _todoitems.FirstOrDefault(td => td.Id == id);
            if(todo == null)return null;
            todo.Title = _todo.Title;
            todo.IsCompleted = _todo.IsCompleted;
            todo.UserId = _todo.UserId;
            return todo;
        }

        public TodoItem Delete(int id)
        {
            var todo = _todoitems.FirstOrDefault(td => td.Id == id);
            if(todo == null)return null;
            _todoitems.Remove(todo);
            return todo;
        }
    }
}