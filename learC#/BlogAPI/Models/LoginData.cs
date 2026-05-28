using System.ComponentModel.DataAnnotations;

namespace Lesson2.Models
{
    public class LoginData
    {
        public required string Username{get; set;}

        [Required]
        public required string Password{get; set;}
    }
}
