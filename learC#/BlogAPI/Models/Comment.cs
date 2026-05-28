namespace Lesson2.Models
{
    public class Comment
    {
        public int Id{get; set;}
        public required string Text{get; set;}
        public DateTime CreatedAt{get; set;}
        public int PostId{get; set;}
        public int UserId{get; set;}
        public Post Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
