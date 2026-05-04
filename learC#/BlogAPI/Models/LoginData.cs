using System.ComponentModel.DataAnnotations;

namespace Lesson2.Models
{
    public class LoginData
    {
        public string Username{get; set;}

        [Required]
        public string Password{get; set;}
    }
}