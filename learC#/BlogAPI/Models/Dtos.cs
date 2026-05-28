using System.ComponentModel.DataAnnotations;

namespace Lesson2.Models
{
    public class RegisterRequest
    {
        [Required, MinLength(3)]
        public required string Username { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }
    }

    public class CreatePostRequest
    {
        [Required, MinLength(3), MaxLength(20)]
        public required string Title { get; set; }

        [Required, MaxLength(2000)]
        public required string Content { get; set; }
    }

    public class UpdatePostRequest
    {
        [Required, MinLength(3), MaxLength(20)]
        public required string Title { get; set; }

        [Required, MaxLength(2000)]
        public required string Content { get; set; }
    }

    public class CreateCommentRequest
    {
        [Required, MaxLength(1000)]
        public required string Text { get; set; }
    }
}
