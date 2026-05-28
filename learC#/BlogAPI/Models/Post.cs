namespace Lesson2.Models
{
    public class Post
    {
        public int Id{get; set;}
        public required string Title{get; set;}
        public required string Content{get; set;}
        public DateTime CreatedAt{get; set;}
        public int UserId{get; set;}
        public User User { get; set; } = null!;
        public List<Comment> Comments { get; set; } = new();
    }
}
