using System.ComponentModel.DataAnnotations;

namespace Lesson2.Models
{
    public class User
    {
        public int Id{get; set;}

        [MinLength(3)]
        public string Username{get; set;}
        public string Email{get; set;}
        public string HashPassword{get; set;}
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}